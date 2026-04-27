using Microsoft.AspNetCore.Mvc;
using VehicleParts.Application.Modules.AdminCore.Interfaces;
using VehicleParts.Application.Modules.CustomerPortal.Interfaces;

namespace VehicleParts.Api.Controllers.AdminCore;

[ApiController]
[Route("api/[controller]")]
public sealed class PartsController : ControllerBase
{
    private readonly IAdminCoreService _adminCoreService;
    private readonly ICustomerPortalService _customerPortalService;

    public PartsController(IAdminCoreService adminCoreService, ICustomerPortalService customerPortalService)
    {
        _adminCoreService = adminCoreService;
        _customerPortalService = customerPortalService;
    }

    [HttpPost]
    public async Task<IActionResult> AddPart(CancellationToken cancellationToken)
    {
        var result = await _adminCoreService.AddPartAsync(cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePart(int id, CancellationToken cancellationToken)
    {
        var result = await _adminCoreService.UpdatePartAsync(id, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePart(int id, CancellationToken cancellationToken)
    {
        var result = await _adminCoreService.DeletePartAsync(id, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllParts(CancellationToken cancellationToken)
    {
        var result = await _adminCoreService.GetAllPartsAsync(cancellationToken);
        return Ok(result);
    }

    [HttpPost("request")]
    public async Task<IActionResult> RequestUnavailablePart(CancellationToken cancellationToken)
    {
        var result = await _customerPortalService.RequestUnavailablePartAsync(cancellationToken);
        return Ok(result);
    }
}
