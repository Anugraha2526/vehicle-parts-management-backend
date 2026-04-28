using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleParts.Application.Modules.Sales.DTOs;
using VehicleParts.Application.Modules.Sales.Interfaces;

namespace VehicleParts.Api.Controllers.Sales;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Staff")]
public sealed class SalesController : ControllerBase
{
    private readonly ISalesService _salesService;
    private readonly ILogger<SalesController> _logger;

    public SalesController(ISalesService salesService, ILogger<SalesController> logger)
    {
        _salesService = salesService;
        _logger = logger;
    }

    /// <summary>
    /// Create a sales invoice for a customer purchase.
    /// Automatically applies a 10% loyalty discount when the subtotal exceeds 5000.
    /// </summary>
    [HttpPost("invoice")]
    public async Task<IActionResult> CreateSalesInvoice(
        [FromBody] CreateSalesInvoiceDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _salesService.CreateSalesInvoiceAsync(request, cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetInvoiceSummary), new { id = result.Data!.InvoiceId }, result);
    }

    /// <summary>Get a summary of a sales invoice by ID (used for email linking).</summary>
    [HttpGet("invoice/{id:guid}")]
    public async Task<IActionResult> GetInvoiceSummary(Guid id, CancellationToken cancellationToken)
    {
        // Placeholder — full retrieval handled in F11 email branch
        return Ok(new { InvoiceId = id, Message = "Invoice found." });
    }

    /// <summary>Send the sales invoice via email to the customer.</summary>
    [HttpPost("invoice/{id:guid}/email")]
    public async Task<IActionResult> SendInvoiceEmail(Guid id, CancellationToken cancellationToken)
    {
        var result = await _salesService.SendInvoiceEmailAsync(id, cancellationToken);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }
}

