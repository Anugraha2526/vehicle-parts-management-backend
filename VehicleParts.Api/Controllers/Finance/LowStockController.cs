using Microsoft.AspNetCore.Mvc;

namespace VehicleParts.Api.Controllers.Finance;

[ApiController]
[Route("api/[controller]")]
public sealed class LowStockController : ControllerBase
{
    // TODO(Member 2): implement low-stock scan and admin notifications.
    [HttpGet("alerts")]
    public IActionResult GetLowStockAlerts([FromQuery] bool scan = true, [FromQuery] int threshold = 10)
    {
        return StatusCode(StatusCodes.Status501NotImplemented,
            "Architecture phase only. Member 2 Finance implementation moved to Member2_Finance_Backup.");
    }

    [HttpPost("alerts/{alertId:guid}/acknowledge")]
    public IActionResult AcknowledgeLowStockAlert(Guid alertId)
    {
        return StatusCode(StatusCodes.Status501NotImplemented,
            "Architecture phase only. Member 2 Finance implementation moved to Member2_Finance_Backup.");
    }
}
