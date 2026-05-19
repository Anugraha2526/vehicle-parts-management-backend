using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleParts.Application.Modules.CustomerPortal.Interfaces;

namespace VehicleParts.Api.Controllers.AdminCore;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Staff")]
public sealed class AdminPartRequestsController : ControllerBase
{
    private readonly ICustomerPortalService _portalService;

    public AdminPartRequestsController(ICustomerPortalService portalService)
    {
        _portalService = portalService;
    }

    // returns all pending requests so the admin can see what customers are looking for
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _portalService.GetAllPendingPartRequestsAsync(cancellationToken);
        return Ok(result);
    }

    // marks the request as approved and removes it from the pending list
    [HttpPatch("{id}/approve")]
    public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken)
    {
        var result = await _portalService.UpdatePartRequestStatusAsync(id, "Approved", cancellationToken);
        if (!result.Success) return NotFound(result.Message);
        return Ok(result);
    }

    // marks the request as rejected and removes it from the pending list
    [HttpPatch("{id}/reject")]
    public async Task<IActionResult> Reject(Guid id, CancellationToken cancellationToken)
    {
        var result = await _portalService.UpdatePartRequestStatusAsync(id, "Rejected", cancellationToken);
        if (!result.Success) return NotFound(result.Message);
        return Ok(result);
    }

}
