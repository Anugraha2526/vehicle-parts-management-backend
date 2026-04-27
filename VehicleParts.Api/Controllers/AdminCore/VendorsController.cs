using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VehicleParts.Application.Modules.AdminCore.DTOs;
using VehicleParts.Application.Modules.AdminCore.Interfaces;

namespace VehicleParts.Api.Controllers.AdminCore;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public sealed class VendorsController : ControllerBase
{
    private readonly IVendorService _vendorService;
    private readonly ILogger<VendorsController> _logger;

    public VendorsController(IVendorService vendorService, ILogger<VendorsController> logger)
    {
        _vendorService = vendorService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllVendors(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _vendorService.GetAllVendorsAsync(cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "error retrieving all vendors");
            return StatusCode(500, "An unexpected error occurred. Please try again.");
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetVendorById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _vendorService.GetVendorByIdAsync(id, cancellationToken);
            if (result is null)
            {
                _logger.LogWarning("vendor {Id} not found", id);
                return NotFound($"Vendor with id '{id}' was not found.");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "error retrieving vendor {Id}", id);
            return StatusCode(500, "An unexpected error occurred. Please try again.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateVendor([FromBody] CreateVendorDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _vendorService.CreateVendorAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetVendorById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "error creating vendor");
            return StatusCode(500, "An unexpected error occurred. Please try again.");
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateVendor(Guid id, [FromBody] UpdateVendorDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _vendorService.UpdateVendorAsync(id, dto, cancellationToken);
            if (result is null)
                return NotFound($"Vendor with id '{id}' was not found.");

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("vendor {Id} not found for update", id);
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "error updating vendor {Id}", id);
            return StatusCode(500, "An unexpected error occurred. Please try again.");
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteVendor(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await _vendorService.DeleteVendorAsync(id, cancellationToken);
            if (!deleted)
                return NotFound($"Vendor with id '{id}' was not found.");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "error deleting vendor {Id}", id);
            return StatusCode(500, "An unexpected error occurred. Please try again.");
        }
    }
}
