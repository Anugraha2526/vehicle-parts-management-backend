using VehicleParts.Application.Common.Models;

namespace VehicleParts.Application.Modules.Identity.Interfaces;

public interface IAuthService
{
    Task<ServiceResult<string>> LoginAsync(CancellationToken cancellationToken = default);
    Task<ServiceResult> RegisterAsync(CancellationToken cancellationToken = default);
}
