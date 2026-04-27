using Microsoft.AspNetCore.Mvc;

namespace VehicleParts.Api.Controllers.Finance;

[ApiController]
[Route("api/[controller]")]
public sealed class ReportsController : ControllerBase
{
    // TODO(Member 2): implement daily/monthly/yearly financial reports.
    [HttpGet("financial")]
    public IActionResult GetFinancialReports([FromQuery] string type)
    {
        return StatusCode(StatusCodes.Status501NotImplemented,
            "Architecture phase only. Member 2 Finance implementation moved to Member2_Finance_Backup.");
    }
}
