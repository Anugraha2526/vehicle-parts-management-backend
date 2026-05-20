using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VehicleParts.Application.Modules.CustomerPortal.DTOs;
using VehicleParts.Application.Modules.CustomerPortal.Interfaces;

namespace VehicleParts.Api.Controllers.CustomerPortal;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Customer")]
public sealed class AppointmentsController : ControllerBase
{
    private readonly ICustomerPortalService _portalService;

    public AppointmentsController(ICustomerPortalService portalService)
    {
        _portalService = portalService;
    }

    // GET /api/Appointments — returns all appointments for the logged-in customer
    [HttpGet]
    public async Task<IActionResult> GetAppointments(CancellationToken cancellationToken)
    {
        var customerId = GetCustomerId();
        if (customerId == Guid.Empty) return Unauthorized();

        var result = await _portalService.GetAppointmentsAsync(customerId, cancellationToken);
        return Ok(result);
    }

    // POST /api/Appointments — books a new appointment
    [HttpPost]
    public async Task<IActionResult> BookAppointment(
        [FromBody] BookAppointmentDto request,
        CancellationToken cancellationToken)
    {
        var customerId = GetCustomerId();
        if (customerId == Guid.Empty) return Unauthorized();

        var result = await _portalService.BookAppointmentAsync(customerId, request, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    private Guid GetCustomerId()
    {
        var raw = User.FindFirstValue("nameid") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(raw, out var id) ? id : Guid.Empty;
    }
}
