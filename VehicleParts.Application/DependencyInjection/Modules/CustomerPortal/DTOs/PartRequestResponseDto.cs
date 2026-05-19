namespace VehicleParts.Application.Modules.CustomerPortal.DTOs;

public sealed class PartRequestResponseDto
{
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }
    public string RequestedPartName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;

    // Pending | Approved | Rejected
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
}
