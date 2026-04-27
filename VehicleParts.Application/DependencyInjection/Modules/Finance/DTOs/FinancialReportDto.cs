namespace VehicleParts.Application.Modules.Finance.DTOs;

public sealed class FinancialReportDto
{
    public string PeriodType { get; init; } = string.Empty;
    public DateTime PeriodStartUtc { get; init; }
    public DateTime PeriodEndUtc { get; init; }

    public int PurchaseInvoiceCount { get; init; }
    public decimal TotalPurchaseAmount { get; init; }

    public int SalesInvoiceCount { get; init; }
    public decimal TotalSalesAmount { get; init; }

    public decimal NetAmount { get; init; }
}
