namespace VehicleParts.Application.Modules.Finance.DTOs;

public sealed class TopPartMetricDto
{
    public Guid PartId { get; init; }
    public string PartName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal Amount { get; init; }
}
