using Microsoft.AspNetCore.Mvc;

namespace vehicle_parts_management_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        [HttpGet("financial")]
        public IActionResult GetFinancialReports([FromQuery] string type) => Ok();

        [HttpGet("customer")]
        public IActionResult GetCustomerReports([FromQuery] string type) => Ok();
    }
}
