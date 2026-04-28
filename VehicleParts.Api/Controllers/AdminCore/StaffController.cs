using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VehicleParts.Application.Modules.AdminCore.DTOs;
using VehicleParts.Application.Modules.AdminCore.Interfaces;

namespace VehicleParts.Api.Controllers.AdminCore;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public sealed class StaffController : ControllerBase
{
    private readonly IStaffService _staffService;
    private readonly ILogger<StaffController> _logger;

    public StaffController(IStaffService staffService, ILogger<StaffController> logger)
    {
        _staffService = staffService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllStaff(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _staffService.GetAllStaffAsync(cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "error retrieving all staff");
            return StatusCode(500, "An unexpected error occurred. Please try again.");
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetStaffById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _staffService.GetStaffByIdAsync(id, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("staff member {Id} not found", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "error retrieving staff member {Id}", id);
            return StatusCode(500, "An unexpected error occurred. Please try again.");
        }
    }

    // register new staff and return 201 with location header
    [HttpPost]
    public async Task<IActionResult> RegisterStaff([FromBody] RegisterStaffDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _staffService.RegisterStaffAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetStaffById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "error registering staff member");
            return StatusCode(500, "An unexpected error occurred. Please try again.");
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateStaff(Guid id, [FromBody] UpdateStaffDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _staffService.UpdateStaffAsync(id, dto, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("staff member {Id} not found for update", id);
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "error updating staff member {Id}", id);
            return StatusCode(500, "An unexpected error occurred. Please try again.");
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteStaff(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _staffService.DeleteStaffAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("staff member {Id} not found for deletion", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "error deleting staff member {Id}", id);
            return StatusCode(500, "An unexpected error occurred. Please try again.");
        }
    }

    // flip active status without needing a full update request
    [HttpPatch("{id:guid}/toggle-status")]
    public async Task<IActionResult> ToggleStatus(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _staffService.ToggleActiveStatusAsync(id, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("staff member {Id} not found for toggle", id);
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "error toggling staff member {Id} status", id);
            return StatusCode(500, "An unexpected error occurred. Please try again.");
        }
    }
}
