using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VehicleParts.Application.Modules.CustomerPortal.Interfaces;

namespace VehicleParts.Api.Controllers.CustomerPortal;

[Authorize(Roles = "Customer")]
[ApiController]
[Route("api/[controller]")]
public sealed class ServiceHistoryController : ControllerBase
{
    private readonly ICustomerPortalService _portal;

    public ServiceHistoryController(ICustomerPortalService portal)
    {
        _portal = portal;
    }

    [HttpGet]
    public async Task<IActionResult> GetServiceHistory(CancellationToken ct)
    {
        var customerId = GetCustomerId();
        if (customerId == Guid.Empty) return Unauthorized();

        var result = await _portal.GetServiceHistoryAsync(customerId, ct);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    private Guid GetCustomerId() =>
        Guid.TryParse(User.FindFirstValue("nameid") ?? User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : Guid.Empty;
}
