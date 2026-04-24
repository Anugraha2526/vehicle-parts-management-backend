using Microsoft.AspNetCore.Mvc;

namespace vehicle_parts_management_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PartsController : ControllerBase
    {
        [HttpPost]
        public IActionResult AddPart() => Ok();

        [HttpPut("{id}")]
        public IActionResult UpdatePart(int id) => Ok();

        [HttpDelete("{id}")]
        public IActionResult DeletePart(int id) => Ok();

        [HttpGet]
        public IActionResult GetAllParts() => Ok();

        [HttpPost("request")]
        public IActionResult RequestUnavailablePart() => Ok();
    }
}
