using VehicleParts.Application.Common.Models;

namespace VehicleParts.Application.Modules.CustomerCRM.Interfaces;

public interface ICustomerCrmService
{
    Task<ServiceResult> RegisterCustomerAsync(CancellationToken cancellationToken = default);
    Task<ServiceResult> SearchCustomersAsync(string query, CancellationToken cancellationToken = default);
    Task<ServiceResult> GetCustomerHistoryAsync(int id, CancellationToken cancellationToken = default);
    Task<ServiceResult> UpdateProfileAsync(int id, CancellationToken cancellationToken = default);
}
