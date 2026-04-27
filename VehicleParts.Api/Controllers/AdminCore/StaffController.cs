using Microsoft.AspNetCore.Mvc;
using VehicleParts.Application.Modules.AdminCore.Interfaces;

namespace VehicleParts.Api.Controllers.AdminCore;

[ApiController]
[Route("api/[controller]")]
public sealed class StaffController : ControllerBase
{
    private readonly IAdminCoreService _adminCoreService;

    public StaffController(IAdminCoreService adminCoreService)
    {
        _adminCoreService = adminCoreService;
    }

    [HttpPost]
    public async Task<IActionResult> RegisterStaff(CancellationToken cancellationToken)
    {
        var result = await _adminCoreService.RegisterStaffAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllStaff(CancellationToken cancellationToken)
    {
        var result = await _adminCoreService.GetAllStaffAsync(cancellationToken);
        return Ok(result);
    }
}
