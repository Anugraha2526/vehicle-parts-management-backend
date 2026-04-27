using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using vehicle_parts_management_backend.Application.DTOs.StaffDTOs;
using vehicle_parts_management_backend.Application.Interfaces;

namespace vehicle_parts_management_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        // register a new staff member, only admin can do this
        [HttpPost]
        public async Task<IActionResult> RegisterStaff([FromBody] RegisterStaffDto dto)
        {
            var result = await _staffService.RegisterStaffAsync(dto);
            return CreatedAtAction(nameof(GetStaffById), new { id = result.Id }, result);
        }

        // get all staff members in the system
        [HttpGet]
        public async Task<IActionResult> GetAllStaff()
        {
            var result = await _staffService.GetAllStaffAsync();
            return Ok(result);
        }

        // get one staff member using their id
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetStaffById(Guid id)
        {
            var result = await _staffService.GetStaffByIdAsync(id);
            return Ok(result);
        }

        // update a staff member's name, role, or active status
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateStaff(Guid id, [FromBody] UpdateStaffDto dto)
        {
            var result = await _staffService.UpdateStaffAsync(id, dto);
            return Ok(result);
        }

        // permanently delete a staff member from the system
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteStaff(Guid id)
        {
            await _staffService.DeleteStaffAsync(id);
            return NoContent();
        }

        // switch a staff member between active and inactive
        [HttpPatch("{id:guid}/toggle-status")]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            var result = await _staffService.ToggleActiveStatusAsync(id);
            return Ok(result);
        }
    }
}
