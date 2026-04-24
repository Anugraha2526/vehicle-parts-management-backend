using Microsoft.AspNetCore.Mvc;

namespace vehicle_parts_management_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StaffController : ControllerBase
    {
        [HttpPost]
        public IActionResult RegisterStaff() => Ok();

        [HttpGet]
        public IActionResult GetAllStaff() => Ok();
    }
}
