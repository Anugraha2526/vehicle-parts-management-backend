namespace VehicleParts.Application.Modules.CustomerPortal.DTOs;

public sealed class ReviewResponseDto
{
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public int Rating { get; init; }
    public string ServiceType { get; init; } = string.Empty;
    public string Comment { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
}
