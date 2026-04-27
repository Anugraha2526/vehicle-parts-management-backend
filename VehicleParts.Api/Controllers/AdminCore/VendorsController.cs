using Microsoft.AspNetCore.Mvc;
using VehicleParts.Application.Modules.AdminCore.Interfaces;

namespace VehicleParts.Api.Controllers.AdminCore;

[ApiController]
[Route("api/[controller]")]
public sealed class VendorsController : ControllerBase
{
    private readonly IAdminCoreService _adminCoreService;

    public VendorsController(IAdminCoreService adminCoreService)
    {
        _adminCoreService = adminCoreService;
    }

    [HttpPost]
    public async Task<IActionResult> AddVendor(CancellationToken cancellationToken)
    {
        var result = await _adminCoreService.AddVendorAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetVendors(CancellationToken cancellationToken)
    {
        var result = await _adminCoreService.GetVendorsAsync(cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateVendor(int id, CancellationToken cancellationToken)
    {
        var result = await _adminCoreService.UpdateVendorAsync(id, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVendor(int id, CancellationToken cancellationToken)
    {
        var result = await _adminCoreService.DeleteVendorAsync(id, cancellationToken);
        return Ok(result);
    }
}
