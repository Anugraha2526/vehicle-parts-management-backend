namespace VehicleParts.Application.Modules.Finance.DTOs;

public sealed class PurchaseInvoiceResponseDto
{
    public Guid InvoiceId { get; init; }
    public string InvoiceNumber { get; init; } = string.Empty;
    public DateTime PurchasedAtUtc { get; init; }
    public decimal TotalAmount { get; init; }
    public int TotalItems { get; init; }
}
