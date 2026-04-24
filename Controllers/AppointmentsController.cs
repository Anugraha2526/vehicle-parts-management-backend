using Microsoft.AspNetCore.Mvc;

namespace vehicle_parts_management_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        [HttpPost]
        public IActionResult BookAppointment() => Ok();

        [HttpGet]
        public IActionResult GetAppointments() => Ok();
    }
}
