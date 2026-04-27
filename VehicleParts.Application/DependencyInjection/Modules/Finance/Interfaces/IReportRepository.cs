using VehicleParts.Application.Modules.Finance.DTOs;

namespace VehicleParts.Application.Modules.Finance.Interfaces;

public interface IReportRepository
{
    Task<FinancialReportDto> GetFinancialReportAsync(
        string type,
        CancellationToken cancellationToken = default);
}
