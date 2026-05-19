using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace VehicleParts.Api.Auth;

/// <summary>
/// Development-only auth fallback for local testing when real JWT login is not wired yet.
/// Accepts a known token and grants Admin role claims.
/// </summary>
public sealed class DevBypassAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "DevBypass";
    private const string DevToken = "dev-bypass-token";
    private const string LegacyDevToken = "dev123";

    public DevBypassAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("Authorization", out var rawHeader))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var header = rawHeader.ToString().Trim();
        var token = header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
            ? header.Substring("Bearer ".Length).Trim()
            : header;

        if (!string.Equals(token, DevToken, StringComparison.Ordinal) &&
            !string.Equals(token, LegacyDevToken, StringComparison.Ordinal))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "dev-admin-user"),
            new(ClaimTypes.Name, "Development Admin"),
            new(ClaimTypes.Role, "Admin")
        };

        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
