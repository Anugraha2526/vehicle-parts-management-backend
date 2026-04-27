using Microsoft.AspNetCore.Mvc;
using VehicleParts.Application.Modules.Sales.Interfaces;

namespace VehicleParts.Api.Controllers.Sales;

[ApiController]
[Route("api/[controller]")]
public sealed class SalesController : ControllerBase
{
    private readonly ISalesService _salesService;

    public SalesController(ISalesService salesService)
    {
        _salesService = salesService;
    }

    [HttpPost("invoice")]
    public async Task<IActionResult> CreateSalesInvoice(CancellationToken cancellationToken)
    {
        var result = await _salesService.CreateSalesInvoiceAsync(cancellationToken);
        return Ok(result);
    }

    [HttpPost("invoice/{id}/email")]
    public async Task<IActionResult> SendInvoiceEmail(int id, CancellationToken cancellationToken)
    {
        var result = await _salesService.SendInvoiceEmailAsync(id, cancellationToken);
        return Ok(result);
    }
}
