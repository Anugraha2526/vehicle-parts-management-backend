using Microsoft.AspNetCore.Mvc;
using VehicleParts.Application.Modules.CustomerPortal.Interfaces;

namespace VehicleParts.Api.Controllers.CustomerPortal;

[ApiController]
[Route("api/[controller]")]
public sealed class AppointmentsController : ControllerBase
{
    private readonly ICustomerPortalService _customerPortalService;

    public AppointmentsController(ICustomerPortalService customerPortalService)
    {
        _customerPortalService = customerPortalService;
    }

    [HttpPost]
    public async Task<IActionResult> BookAppointment(CancellationToken cancellationToken)
    {
        var result = await _customerPortalService.BookAppointmentAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAppointments(CancellationToken cancellationToken)
    {
        var result = await _customerPortalService.GetAppointmentsAsync(cancellationToken);
        return Ok(result);
    }
}
