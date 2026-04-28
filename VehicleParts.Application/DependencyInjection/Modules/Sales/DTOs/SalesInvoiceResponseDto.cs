namespace VehicleParts.Application.Modules.Sales.DTOs;

public sealed class SalesInvoiceResponseDto
{
    public Guid InvoiceId { get; init; }
    public string InvoiceNumber { get; init; } = string.Empty;
    public Guid CustomerId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public Guid StaffId { get; init; }
    public DateTime SoldAtUtc { get; init; }
    public decimal SubTotal { get; init; }
    public bool LoyaltyDiscountApplied { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal TotalAmount { get; init; }
    public List<SalesInvoiceItemResponseDto> Items { get; init; } = [];
}
