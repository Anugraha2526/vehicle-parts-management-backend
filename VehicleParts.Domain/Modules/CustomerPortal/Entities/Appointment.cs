using VehicleParts.Domain.Common;

namespace VehicleParts.Domain.Modules.CustomerPortal.Entities;

public sealed class Appointment : BaseEntity
{
    public Guid CustomerId { get; set; }
    public DateTime AppointmentAtUtc { get; set; }
    public string Notes { get; set; } = string.Empty;
}
