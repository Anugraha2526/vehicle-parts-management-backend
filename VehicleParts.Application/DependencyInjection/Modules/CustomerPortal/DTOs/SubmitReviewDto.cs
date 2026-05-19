namespace VehicleParts.Application.Modules.CustomerPortal.DTOs;

public sealed class SubmitReviewDto
{
    // Must be between 1 and 5
    public int Rating { get; init; }
    public string ServiceType { get; init; } = string.Empty;
    public string Comment { get; init; } = string.Empty;
}
