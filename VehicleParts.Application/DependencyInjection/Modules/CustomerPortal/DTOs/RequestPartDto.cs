namespace VehicleParts.Application.Modules.CustomerPortal.DTOs;

public sealed class RequestPartDto
{
    public string RequestedPartName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}
