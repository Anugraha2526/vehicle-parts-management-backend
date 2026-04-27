using VehicleParts.Application.Common.Models;
using VehicleParts.Application.Modules.Sales.Interfaces;

namespace VehicleParts.Application.Modules.Sales.Services;

public sealed class SalesService : ISalesService
{
    public Task<ServiceResult> CreateSalesInvoiceAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(ServiceResult.Ok("Sales invoice use case is wired."));

    public Task<ServiceResult> SendInvoiceEmailAsync(int id, CancellationToken cancellationToken = default) =>
        Task.FromResult(ServiceResult.Ok($"Invoice email use case is wired for invoice id {id}."));
}
