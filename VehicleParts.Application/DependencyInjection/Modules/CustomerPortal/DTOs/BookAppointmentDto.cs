namespace VehicleParts.Application.Modules.CustomerPortal.DTOs;

public sealed class BookAppointmentDto
{
    public DateTime AppointmentAtUtc { get; init; }
    public string ServiceType { get; init; } = string.Empty;
    public string Notes { get; init; } = string.Empty;
}
