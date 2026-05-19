using VehicleParts.Application.Common.Models;
using VehicleParts.Application.Modules.Finance.DTOs;

namespace VehicleParts.Application.Modules.Finance.Interfaces;

public interface IPurchaseService
{
    Task<ServiceResult<PurchaseInvoiceResponseDto>> CreatePurchaseInvoiceAsync(
        CreatePurchaseInvoiceDto request,
        CancellationToken cancellationToken = default);

    Task<ServiceResult<IReadOnlyList<PurchaseInvoiceResponseDto>>> GetPurchaseInvoicesAsync(
        CancellationToken cancellationToken = default);

    Task<ServiceResult<PurchaseInvoiceDetailResponseDto>> GetPurchaseInvoiceByIdAsync(
        Guid invoiceId,
        CancellationToken cancellationToken = default);
}
