using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using VehicleParts.Application.Common.Models;
using VehicleParts.Application.Modules.Finance.DTOs;
using VehicleParts.Application.Modules.Finance.Interfaces;

namespace VehicleParts.Api.Controllers.Finance;

[Authorize(Roles = "Admin,Staff")]
[ApiController]
[Route("api/[controller]")]
[Route("api/finance/reports")]
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
        [FromQuery] string? date,
        CancellationToken cancellationToken)
    {
        DateTime? referenceDateUtc = null;
        if (!string.IsNullOrWhiteSpace(date))
        {
            var parsed = DateTime.TryParse(
                date,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out var parsedDate);

            if (!parsed)
            {
                return BadRequest(ServiceResult<FinancialReportDto>.Fail(
                    "Invalid date value. Use ISO format like 2026-05-19."));
            }

            referenceDateUtc = parsedDate;
        }

        var result = await _reportService.GetFinancialReportAsync(type, referenceDateUtc, cancellationToken);
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

