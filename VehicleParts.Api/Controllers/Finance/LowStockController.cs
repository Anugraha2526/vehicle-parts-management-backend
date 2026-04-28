using Microsoft.AspNetCore.Mvc;
using VehicleParts.Application.Modules.Finance.Interfaces;

namespace VehicleParts.Api.Controllers.Finance;

[ApiController]
[Route("api/[controller]")]
public sealed class LowStockController : ControllerBase
{
    private readonly ILowStockService _lowStockService;

    public LowStockController(ILowStockService lowStockService)
    {
        _lowStockService = lowStockService;
    }

    [HttpGet("alerts")]
    public async Task<IActionResult> GetLowStockAlerts(
        [FromQuery] bool scan = true,
        [FromQuery] int threshold = 10,
        CancellationToken cancellationToken = default)
    {
        var result = scan
            ? await _lowStockService.ScanAndNotifyLowStockAsync(threshold, cancellationToken)
            : await _lowStockService.GetActiveAlertsAsync(cancellationToken);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("alerts/{alertId:guid}/acknowledge")]
    public async Task<IActionResult> AcknowledgeLowStockAlert(Guid alertId, CancellationToken cancellationToken)
    {
        var result = await _lowStockService.AcknowledgeAlertAsync(alertId, cancellationToken);
        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }
}
