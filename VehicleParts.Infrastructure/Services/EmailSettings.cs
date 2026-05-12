namespace VehicleParts.Infrastructure.Services;

/// <summary>
/// Bound to the "EmailSettings" section in appsettings.json.
/// </summary>
public sealed class EmailSettings
{
    public string Host      { get; init; } = string.Empty;
    public int    Port      { get; init; } = 587;
    public string Username  { get; init; } = string.Empty;
    public string Password  { get; init; } = string.Empty;
    public string FromEmail { get; init; } = string.Empty;
    public string FromName  { get; init; } = string.Empty;
}
