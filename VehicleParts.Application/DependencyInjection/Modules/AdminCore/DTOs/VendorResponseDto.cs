namespace VehicleParts.Application.Modules.AdminCore.DTOs;

public sealed class VendorResponseDto
{
    public Guid Id { get; init; }
    public string VendorName { get; init; } = string.Empty;
    public string ContactPerson { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Address { get; init; }
    public string? Notes { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime UpdatedAtUtc { get; init; }
}
