using Microsoft.AspNetCore.Mvc;
using VehicleParts.Application.Modules.Identity.Interfaces;

namespace VehicleParts.Api.Controllers.Identity;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(cancellationToken);
        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> SelfRegister(CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(cancellationToken);
        return Ok(result);
    }
}
