namespace VehicleParts.Application.Modules.Finance.DTOs;

public sealed class CreatePurchaseInvoiceDto
{
    public Guid VendorId { get; init; }
    public DateTime? PurchasedAtUtc { get; init; }
    public IReadOnlyList<PurchaseInvoiceItemDto> Items { get; init; } = Array.Empty<PurchaseInvoiceItemDto>();
}
