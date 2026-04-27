using VehicleParts.Domain.Common;

namespace VehicleParts.Domain.Modules.Finance.Entities;

public sealed class LowStockNotification : BaseEntity
{
    public Guid PartId { get; set; }
    public string PartName { get; set; } = string.Empty;
    public int CurrentStockQuantity { get; set; }
    public int Threshold { get; set; } = 10;
    public bool IsAcknowledged { get; set; }
    public DateTime NotifiedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? AcknowledgedAtUtc { get; set; }
}
