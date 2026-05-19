using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VehicleParts.Application.Modules.CustomerPortal.DTOs;
using VehicleParts.Application.Modules.CustomerPortal.Interfaces;

namespace VehicleParts.Api.Controllers.CustomerPortal;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Customer")]
public sealed class PartRequestsController : ControllerBase
{
    private readonly ICustomerPortalService _portalService;

    public PartRequestsController(ICustomerPortalService portalService)
    {
        _portalService = portalService;
    }

    // GET /api/PartRequests — returns all part requests submitted by the logged-in customer
    [HttpGet]
    public async Task<IActionResult> GetPartRequests(CancellationToken cancellationToken)
    {
        var customerId = GetCustomerId();
        if (customerId == Guid.Empty) return Unauthorized();

        var result = await _portalService.GetPartRequestsAsync(customerId, cancellationToken);
        return Ok(result);
    }

    // POST /api/PartRequests — submits a request for an unavailable part
    [HttpPost]
    public async Task<IActionResult> RequestPart(
        [FromBody] RequestPartDto request,
        CancellationToken cancellationToken)
    {
        var customerId = GetCustomerId();
        if (customerId == Guid.Empty) return Unauthorized();

        var result = await _portalService.RequestUnavailablePartAsync(customerId, request, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    private Guid GetCustomerId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(raw, out var id) ? id : Guid.Empty;
    }
}
