using VehicleParts.Application.Common.Models;
using VehicleParts.Application.Modules.CustomerCRM.Interfaces;

namespace VehicleParts.Application.Modules.CustomerCRM.Services;

public sealed class CustomerCrmService : ICustomerCrmService
{
    public Task<ServiceResult> RegisterCustomerAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(ServiceResult.Ok("Customer registration flow is wired."));

    public Task<ServiceResult> SearchCustomersAsync(string query, CancellationToken cancellationToken = default) =>
        Task.FromResult(ServiceResult.Ok($"Customer search use case is wired for query '{query}'."));

    public Task<ServiceResult> GetCustomerHistoryAsync(int id, CancellationToken cancellationToken = default) =>
        Task.FromResult(ServiceResult.Ok($"Customer history use case is wired for id {id}."));

    public Task<ServiceResult> UpdateProfileAsync(int id, CancellationToken cancellationToken = default) =>
        Task.FromResult(ServiceResult.Ok($"Customer profile update use case is wired for id {id}."));
}
