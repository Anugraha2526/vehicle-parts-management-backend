using VehicleParts.Application.Common.Models;
using VehicleParts.Application.DTOs;
using VehicleParts.Application.Modules.Finance.DTOs;

namespace VehicleParts.Application.Modules.Finance.Interfaces;

public interface IReportService
{
    Task<ServiceResult<FinancialReportDto>> GetFinancialReportAsync(
        string type,
        DateTime? referenceDateUtc = null,
        CancellationToken cancellationToken = default);

    Task<DashboardSummaryDto> GetDashboardSummaryAsync(CancellationToken cancellationToken = default);
}
