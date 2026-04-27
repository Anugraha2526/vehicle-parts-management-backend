using VehicleParts.Application.Common.Models;
using VehicleParts.Application.Modules.Identity.Interfaces;

namespace VehicleParts.Application.Modules.Identity.Services;

public sealed class AuthService : IAuthService
{
    public Task<ServiceResult<string>> LoginAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(ServiceResult<string>.Ok("login-placeholder-token", "Login use case is wired through Application layer."));
    }

    public Task<ServiceResult> RegisterAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(ServiceResult.Ok("Self-register use case is wired through Application layer."));
    }
}
