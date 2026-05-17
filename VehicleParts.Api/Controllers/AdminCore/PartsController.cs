using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VehicleParts.Application.Modules.AdminCore.DTOs;
using VehicleParts.Application.Modules.AdminCore.Interfaces;

namespace VehicleParts.Api.Controllers.AdminCore;

// REST controller for parts inventory management — Admin only
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public sealed class PartsController : ControllerBase
{
    private readonly IPartsService _partsService;
    private readonly ILogger<PartsController> _logger;

    public PartsController(IPartsService partsService, ILogger<PartsController> logger)
    {
        _partsService = partsService;
        _logger = logger;
    }

    // return all parts sorted by most recently added
    [HttpGet]
    public async Task<IActionResult> GetAllParts(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _partsService.GetAllPartsAsync(cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "error retrieving all parts");
            return StatusCode(500, "An unexpected error occurred. Please try again.");
        }
    }

    // declared before {id:guid} to avoid ASP.NET routing the literal "low-stock" as a guid
    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStockParts(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _partsService.GetLowStockPartsAsync(ct: cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "error retrieving low stock parts");
            return StatusCode(500, "An unexpected error occurred. Please try again.");
        }
    }

    // return single part or 404 if the id does not exist
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPartById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _partsService.GetPartByIdAsync(id, cancellationToken);
            if (result is null)
            {
                _logger.LogWarning("part {Id} not found", id);
                return NotFound($"Part with id '{id}' was not found.");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "error retrieving part {Id}", id);
            return StatusCode(500, "An unexpected error occurred. Please try again.");
        }
    }

    // create new part and return 201 with location header
    [HttpPost]
    public async Task<IActionResult> CreatePart([FromBody] CreatePartDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _partsService.CreatePartAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetPartById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "error creating part");
            return StatusCode(500, "An unexpected error occurred. Please try again.");
        }
    }

    // update only fields provided in the request body
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdatePart(Guid id, [FromBody] UpdatePartDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _partsService.UpdatePartAsync(id, dto, cancellationToken);
            if (result is null)
                return NotFound($"Part with id '{id}' was not found.");

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("part {Id} not found for update", id);
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "error updating part {Id}", id);
            return StatusCode(500, "An unexpected error occurred. Please try again.");
        }
    }

    // remove part permanently, return 204 on success
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeletePart(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await _partsService.DeletePartAsync(id, cancellationToken);
            if (!deleted)
            {
                _logger.LogWarning("part {Id} not found for deletion", id);
                return NotFound($"Part with id '{id}' was not found.");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "error deleting part {Id}", id);
            return StatusCode(500, "An unexpected error occurred. Please try again.");
        }
    }
}
