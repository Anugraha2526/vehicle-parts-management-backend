using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using vehicle_parts_management_backend.Application.DTOs;
using vehicle_parts_management_backend.Application.Interfaces;

namespace vehicle_parts_management_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public AuthController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCustomerDto dto)
        {
            try
            {
                var userId = await _customerService.RegisterCustomerAsync(dto);
                return Ok(new { UserId = userId, Message = "Registration successful!" });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var response = await _customerService.LoginAsync(dto);
            if (response == null)
            {
                return Unauthorized(new { Message = "Invalid email or password." });
            }
            return Ok(response);
        }
    }
}
