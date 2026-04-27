using VehicleParts.Application.Common.Models;
using VehicleParts.Application.Modules.Finance.DTOs;
using VehicleParts.Application.Modules.Finance.Interfaces;

namespace VehicleParts.Application.Modules.Finance.Services;

public sealed class ReportService : IReportService
{
    private readonly IReportRepository _reportRepository;

    public ReportService(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<ServiceResult<FinancialReportDto>> GetFinancialReportAsync(
        string type,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            return ServiceResult<FinancialReportDto>.Fail("Report type is required. Use daily, monthly, or yearly.");
        }

        try
        {
            var report = await _reportRepository.GetFinancialReportAsync(type, cancellationToken);
            return ServiceResult<FinancialReportDto>.Ok(report, "Financial report generated.");
        }
        catch (ArgumentException ex)
        {
            return ServiceResult<FinancialReportDto>.Fail(ex.Message);
        }
    }
}
