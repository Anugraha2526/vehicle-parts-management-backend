using Microsoft.AspNetCore.Mvc;
using VehicleParts.Application.DTOs.CustomerPortal;
using VehicleParts.Application.Interfaces;

namespace VehicleParts.Api.Controllers.CustomerPortal
{
    [Route("api/customer/part-requests")]
    [ApiController]
    public class PartRequestsController : ControllerBase
    {
        private readonly IPartRequestService _service;

        public PartRequestsController(IPartRequestService service)
        {
            _service = service;
        }

        // ✅ POST
        [HttpPost]
        public async Task<IActionResult> Create(CreatePartRequestDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return Ok(result);
        }

        // ✅ GET by CustomerId
        [HttpGet("{customerId}")]
public async Task<IActionResult> GetByCustomer(Guid customerId)
        {
            var result = await _service.GetByCustomerIdAsync(customerId);
            return Ok(result);
        }
    }
}