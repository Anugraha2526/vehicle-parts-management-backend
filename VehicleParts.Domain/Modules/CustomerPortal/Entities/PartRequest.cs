using VehicleParts.Domain.Common;

namespace VehicleParts.Domain.Modules.CustomerPortal.Entities;

public sealed class PartRequest : BaseEntity
{
    public Guid CustomerId { get; set; }
    public string RequestedPartName { get; set; } = string.Empty;

    // Extra context the customer provides about why they need the part
    public string Description { get; set; } = string.Empty;

    // Pending → Approved | Rejected
    public string Status { get; set; } = "Pending";
}
