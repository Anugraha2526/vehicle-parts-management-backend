using VehicleParts.Domain.Common;

namespace VehicleParts.Domain.Modules.CustomerPortal.Entities;

public sealed class PartRequest : BaseEntity
{
    public Guid CustomerId { get; set; }
    public string RequestedPartName { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
}
