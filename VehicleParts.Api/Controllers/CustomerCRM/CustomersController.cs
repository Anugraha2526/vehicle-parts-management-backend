using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VehicleParts.Application.DTOs;
using VehicleParts.Application.Interfaces;

namespace VehicleParts.Api.Controllers.CustomerCRM
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // Get all customers (Feature 8)
        [HttpGet("all")]
        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await _customerService.SearchCustomersAsync("");
            return Ok(customers);
        }

        // Feature 6 & 12: Register customer (Staff or Self)
        [HttpPost]
        public async Task<IActionResult> RegisterCustomer([FromBody] RegisterCustomerDto dto)
        {
            var customerId = await _customerService.RegisterCustomerAsync(dto);
            return Ok(new { CustomerId = customerId, Message = "Customer registered successfully." });
        }

        // Feature 10: Search customers
        [HttpGet("search")]
        public async Task<IActionResult> SearchCustomers([FromQuery] string? query = null)
        {
            var customers = await _customerService.SearchCustomersAsync(query ?? string.Empty);
            return Ok(customers);
        }

        // Feature 8: Get customer details and history
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerDetails(Guid id)
        {
            var customer = await _customerService.GetCustomerProfileAsync(id);
            if (customer == null) return NotFound("Customer not found.");
            return Ok(customer);
        }

        // Feature 12: Update profile
        [HttpPut("{id}/profile")]
        public async Task<IActionResult> UpdateProfile(Guid id, [FromBody] UpdateProfileDto dto)
        {
            var result = await _customerService.UpdateCustomerProfileAsync(id, dto);
            if (!result) return BadRequest("Could not update profile.");
            return Ok(new { Message = "Profile updated successfully." });
        }

        // Feature 12: Add vehicle
        [HttpPost("{id}/vehicles")]
        public async Task<IActionResult> AddVehicle(Guid id, [FromBody] VehicleDto dto)
        {
            var result = await _customerService.AddVehicleAsync(id, dto);
            if (!result) return BadRequest("Could not add vehicle.");
            return Ok(new { Message = "Vehicle added successfully." });
        }
    }
}
