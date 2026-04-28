using VehicleParts.Domain.Common;

namespace VehicleParts.Domain.Modules.CustomerCRM.Entities;

public sealed class Vehicle : BaseEntity
{
    public Guid CustomerId { get; set; }
    public string VehicleNumber { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
}
