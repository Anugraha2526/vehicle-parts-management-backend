using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace VehicleParts.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ChatController : ControllerBase
{
    // comprehensive prompt so Gemini can answer both vehicle and ChitoSpare questions
    private const string SystemPrompt = """
        You are a vehicle assistant for ChitoSpare, a vehicle parts and service management platform based in Nepal.

        About ChitoSpare:
        - ChitoSpare is a complete management system for vehicle parts shops in Nepal
        - It helps shop owners manage parts inventory, track stock levels, and receive automatic low-stock alerts
        - Shops manage vendor and supplier relationships and record purchase invoices for restocking
        - Customer records link vehicle owners to their registered vehicles and purchase history
        - The platform generates sales invoices, financial reports, and overdue credit reminders for unpaid invoices
        - Admin and staff accounts manage all shop operations from a unified dashboard
        - Customers can log in to view their own profile, registered vehicles, and transaction history
        - ChitoSpare supports loyalty discounts for repeat customers
        - The platform is built for Nepali vehicle parts businesses and uses NPR (Nepali Rupee)

        Answer questions about:
        - Vehicles, their conditions, part failures, diagnostics, and maintenance tips
        - ChitoSpare platform features, how to use them, and what services are offered

        Politely decline anything unrelated with: Sorry, I am designed to answer vehicle-related and platform-related queries only.
        Keep all responses short and precise.
        """;

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ChatController> _logger;

    public ChatController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<ChatController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost("ask")]
    public async Task<IActionResult> Ask([FromBody] ChatRequestDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.Message))
            return BadRequest("Message cannot be empty.");

        var apiKey = _configuration["Gemini:ApiKey"];
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

        // prepend system context to the user message for reliable single-turn Gemini requests
        var fullMessage = $"{SystemPrompt}\n\nUser question: {dto.Message}";

        var requestBody = new
        {
            contents = new[]
            {
                new { parts = new[] { new { text = fullMessage } } }
            }
        };

        try
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync(url, requestBody, ct);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(ct);
                _logger.LogError("Gemini API returned {Status}: {Body}", response.StatusCode, errorBody);
                return StatusCode(502, "AI service is temporarily unavailable.");
            }

            var json = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: ct);
            var reply = json
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString() ?? "Sorry, I could not generate a response.";

            return Ok(new { reply });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "error calling Gemini API");
            return StatusCode(502, "AI service is temporarily unavailable.");
        }
    }
}

public sealed class ChatRequestDto
{
    public string Message { get; set; } = string.Empty;
}
