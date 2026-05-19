using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleParts.Application.Modules.Finance.DTOs;
using VehicleParts.Application.Modules.Finance.Interfaces;

namespace VehicleParts.Api.Controllers.Finance;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Staff")]
[Route("api/finance/purchases")]
public sealed class PurchasesController : ControllerBase
{
    private readonly IPurchaseService _purchaseService;

    public PurchasesController(IPurchaseService purchaseService)
    {
        _purchaseService = purchaseService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPurchaseInvoices(CancellationToken cancellationToken)
    {
        var result = await _purchaseService.GetPurchaseInvoicesAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPurchaseInvoiceById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _purchaseService.GetPurchaseInvoiceByIdAsync(id, cancellationToken);
        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpPost]
    [HttpPost("invoice")]
    public async Task<IActionResult> CreatePurchaseInvoice(
        [FromBody] CreatePurchaseInvoiceDto request,
        CancellationToken cancellationToken)
    {
        var result = await _purchaseService.CreatePurchaseInvoiceAsync(request, cancellationToken);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
