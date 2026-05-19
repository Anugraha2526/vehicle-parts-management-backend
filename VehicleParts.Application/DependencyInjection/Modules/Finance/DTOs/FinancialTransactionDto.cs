namespace VehicleParts.Application.Modules.Finance.DTOs;

public sealed class FinancialTransactionDto
{
    public string EntryType { get; init; } = string.Empty;
    public Guid InvoiceId { get; init; }
    public string InvoiceNumber { get; init; } = string.Empty;
    public DateTime TransactionDateUtc { get; init; }
    public int ItemCount { get; init; }
    public decimal TotalAmount { get; init; }
}
