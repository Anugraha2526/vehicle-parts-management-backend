using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleParts.Application.Modules.Sales.DTOs;
using VehicleParts.Application.Modules.Sales.Interfaces;

namespace VehicleParts.Api.Controllers.Sales;

[Authorize(Roles = "Staff,Admin")]
[ApiController]
[Route("api/[controller]")]
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
    /// Gets a list of recent sales invoices.
    /// </summary>
    [HttpGet("invoice")]
    public async Task<IActionResult> GetRecentInvoices(
        [FromQuery] int limit = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _salesService.GetRecentInvoicesAsync(limit, cancellationToken);
        return Ok(result);
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
    public Task<IActionResult> GetInvoiceSummary(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult<IActionResult>(Ok(new { InvoiceId = id, Message = "Invoice found." }));
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

    /// <summary>Gets a list of all strictly unpaid invoices older than specified months.</summary>
    [HttpGet("invoice/overdue")]
    public async Task<IActionResult> GetOverdueInvoices(
        [FromQuery] int months = 1, CancellationToken cancellationToken = default)
    {
        var result = await _salesService.GetUnpaidInvoicesAsync(months, cancellationToken);
        return Ok(result);
    }

    /// <summary>Reminds all overdue customers securely in batch via email service.</summary>
    [HttpPost("invoice/remind-overdue")]
    public async Task<IActionResult> RemindAllOverdueInvoices(
        [FromQuery] int months = 1, CancellationToken cancellationToken = default)
    {
        var result = await _salesService.SendDueRemindersAsync(months, null, cancellationToken);
        if (!result.Success)
            return BadRequest(result);
            
        return Ok(result);
    }

    /// <summary>Sends a targeted overdue reminder solely to one distinct customer invoice.</summary>
    [HttpPost("invoice/{id:guid}/remind")]
    public async Task<IActionResult> RemindSingleOverdueInvoice(
        Guid id, CancellationToken cancellationToken)
    {
        var result = await _salesService.SendDueRemindersAsync(1, id, cancellationToken);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>Admin action locking the invoice credit permanently as paid.</summary>
    [HttpPost("invoice/{id:guid}/mark-paid")]
    public async Task<IActionResult> MarkInvoiceAsPaid(
        Guid id, CancellationToken cancellationToken)
    {
        var result = await _salesService.MarkInvoiceAsPaidAsync(id, cancellationToken);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>Admin action reverting a mistakenly paid invoice back to pending.</summary>
    [HttpPost("invoice/{id:guid}/mark-unpaid")]
    public async Task<IActionResult> MarkInvoiceAsUnpaid(
        Guid id, CancellationToken cancellationToken)
    {
        var result = await _salesService.MarkInvoiceAsUnpaidAsync(id, cancellationToken);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}



