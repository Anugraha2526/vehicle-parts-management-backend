using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleParts.Application.Modules.Finance.Interfaces;

namespace VehicleParts.Api.Controllers.Finance;

[Authorize(Roles = "Staff,Admin")]
[ApiController]
[Route("api/[controller]")]
public sealed class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("financial")]
    public async Task<IActionResult> GetFinancialReports(
        [FromQuery] string type,
        CancellationToken cancellationToken)
    {
        var result = await _reportService.GetFinancialReportAsync(type, cancellationToken);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetDashboardSummary(CancellationToken cancellationToken)
    {
        var result = await _reportService.GetDashboardSummaryAsync(cancellationToken);
        return Ok(result);
    }
}

