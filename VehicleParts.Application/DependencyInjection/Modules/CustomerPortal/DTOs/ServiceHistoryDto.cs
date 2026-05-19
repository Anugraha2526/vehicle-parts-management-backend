namespace VehicleParts.Application.Modules.CustomerPortal.DTOs;

public sealed class InvoiceItemDto
{
    public string PartName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal SubTotal { get; init; }
}

public sealed class InvoiceResponseDto
{
    public Guid Id { get; init; }
    public string InvoiceNumber { get; init; } = string.Empty;
    public DateTime SoldAtUtc { get; init; }
    public decimal SubTotal { get; init; }
    public bool LoyaltyDiscountApplied { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal TotalAmount { get; init; }
    public bool IsPaid { get; init; }
    public DateTime? PaidAtUtc { get; init; }
    public List<InvoiceItemDto> Items { get; init; } = [];
}

public sealed class ServiceHistoryDto
{
    public decimal TotalSpent { get; init; }
    public int InvoiceCount { get; init; }
    public int PaidCount { get; init; }
    public int UnpaidCount { get; init; }
    public decimal TotalSaved { get; init; }
    public DateTime? LastPurchaseDate { get; init; }
    public List<InvoiceResponseDto> Invoices { get; init; } = [];
}
