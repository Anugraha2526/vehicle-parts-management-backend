namespace VehicleParts.Application.Modules.Finance.DTOs;

public sealed class LowStockAlertDto
{
    public Guid AlertId { get; init; }
    public Guid PartId { get; init; }
    public string PartName { get; init; } = string.Empty;
    public int CurrentStockQuantity { get; init; }
    public int Threshold { get; init; }
    public DateTime NotifiedAtUtc { get; init; }
    public bool IsAcknowledged { get; init; }
}
