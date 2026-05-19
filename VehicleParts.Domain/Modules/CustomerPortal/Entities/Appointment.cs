using VehicleParts.Domain.Common;

namespace VehicleParts.Domain.Modules.CustomerPortal.Entities;

public sealed class Appointment : BaseEntity
{
    public Guid CustomerId { get; set; }
    public DateTime AppointmentAtUtc { get; set; }

    // "Oil Change", "Tire Service", "Brake Inspection", etc.
    public string ServiceType { get; set; } = string.Empty;

    // Pending → Confirmed → Completed | Cancelled
    public string Status { get; set; } = "Pending";

    public string Notes { get; set; } = string.Empty;
}
