using VehicleParts.Application.Common.Models;
using VehicleParts.Application.Modules.Finance.DTOs;

namespace VehicleParts.Application.Modules.Finance.Interfaces;

public interface IReportService
{
    Task<ServiceResult<FinancialReportDto>> GetFinancialReportAsync(
        string type,
        CancellationToken cancellationToken = default);
}
