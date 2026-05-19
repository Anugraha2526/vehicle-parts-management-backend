namespace VehicleParts.Application.Modules.CustomerPortal.DTOs;

public sealed class AppointmentResponseDto
{
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }
    public DateTime AppointmentAtUtc { get; init; }
    public string ServiceType { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Notes { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
}
