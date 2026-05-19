namespace VehicleParts.Application.Modules.Finance.DTOs;

public sealed class PurchaseInvoiceDetailResponseDto
{
    public Guid InvoiceId { get; init; }
    public string InvoiceNumber { get; init; } = string.Empty;
    public Guid VendorId { get; init; }
    public DateTime PurchasedAtUtc { get; init; }
    public decimal TotalAmount { get; init; }
    public int TotalItems { get; init; }
    public IReadOnlyList<PurchaseInvoiceLineItemResponseDto> Items { get; init; } =
        Array.Empty<PurchaseInvoiceLineItemResponseDto>();
}
