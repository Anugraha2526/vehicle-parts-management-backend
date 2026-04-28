using VehicleParts.Application.Common.Models;
using VehicleParts.Application.Modules.Sales.DTOs;

namespace VehicleParts.Application.Modules.Sales.Interfaces;

public interface ISalesService
{
    Task<ServiceResult<SalesInvoiceResponseDto>> CreateSalesInvoiceAsync(
        CreateSalesInvoiceDto request,
        CancellationToken cancellationToken = default);

    Task<ServiceResult> SendInvoiceEmailAsync(Guid invoiceId, CancellationToken cancellationToken = default);
}

