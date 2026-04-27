namespace VehicleParts.Application.Modules.Sales.DTOs;

public sealed class SalesInvoiceItemResponseDto
{
    public Guid PartId { get; init; }
    public string PartName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal SubTotal { get; init; }
}
