using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleParts.Application.Modules.AdminCore.DTOs;
using VehicleParts.Application.Modules.AdminCore.Interfaces;

namespace VehicleParts.Api.Controllers.AdminCore;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public sealed class StaffController : ControllerBase
{
    private readonly IStaffService _staffService;

    public StaffController(IStaffService staffService)
    {
        _staffService = staffService;
    }

    [HttpPost]
    public async Task<IActionResult> RegisterStaff([FromBody] RegisterStaffDto dto, CancellationToken cancellationToken)
    {
        var result = await _staffService.RegisterStaffAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetStaffById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllStaff(CancellationToken cancellationToken)
    {
        var result = await _staffService.GetAllStaffAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetStaffById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _staffService.GetStaffByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateStaff(Guid id, [FromBody] UpdateStaffDto dto, CancellationToken cancellationToken)
    {
        var result = await _staffService.UpdateStaffAsync(id, dto, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteStaff(Guid id, CancellationToken cancellationToken)
    {
        await _staffService.DeleteStaffAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id:guid}/toggle-status")]
    public async Task<IActionResult> ToggleStatus(Guid id, CancellationToken cancellationToken)
    {
        var result = await _staffService.ToggleActiveStatusAsync(id, cancellationToken);
        return Ok(result);
    }
}
