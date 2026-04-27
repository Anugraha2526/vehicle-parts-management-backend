using VehicleParts.Application.Common.Models;
using VehicleParts.Application.Modules.Finance.DTOs;
using VehicleParts.Application.Modules.Finance.Interfaces;

namespace VehicleParts.Application.Modules.Finance.Services;

public sealed class ReportService : IReportService
{
    public Task<ServiceResult<FinancialReportDto>> GetFinancialReportAsync(
        string type,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            ServiceResult<FinancialReportDto>.Fail(
                "Temporarily disabled. Member 2 implementation is parked in Member2_Finance_Backup."));
    }
}
