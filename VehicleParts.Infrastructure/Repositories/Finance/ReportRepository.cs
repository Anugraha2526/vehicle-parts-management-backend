using VehicleParts.Application.Modules.Finance.DTOs;
using VehicleParts.Application.Modules.Finance.Interfaces;

namespace VehicleParts.Infrastructure.Repositories.Finance;

public sealed class ReportRepository : IReportRepository
{
    public Task<FinancialReportDto> GetFinancialReportAsync(
        string type,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Architecture skeleton only. Implement Member 2 finance repository later.");
    }
}
