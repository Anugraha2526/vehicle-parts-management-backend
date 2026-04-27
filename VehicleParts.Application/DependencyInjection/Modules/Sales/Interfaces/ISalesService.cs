using VehicleParts.Application.Common.Models;

namespace VehicleParts.Application.Modules.Sales.Interfaces;

public interface ISalesService
{
    Task<ServiceResult> CreateSalesInvoiceAsync(CancellationToken cancellationToken = default);
    Task<ServiceResult> SendInvoiceEmailAsync(int id, CancellationToken cancellationToken = default);
}
