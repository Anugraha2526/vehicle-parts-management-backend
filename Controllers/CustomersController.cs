using Microsoft.AspNetCore.Mvc;

namespace vehicle_parts_management_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        [HttpPost]
        public IActionResult RegisterCustomer() => Ok();

        [HttpGet]
        public IActionResult SearchCustomers([FromQuery] string query) => Ok();

        [HttpGet("{id}/history")]
        public IActionResult GetCustomerHistory(int id) => Ok();

        [HttpPut("{id}")]
        public IActionResult UpdateProfile(int id) => Ok();
    }
}
