using Microsoft.AspNetCore.Mvc;

namespace vehicle_parts_management_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendorsController : ControllerBase
    {
        [HttpPost]
        public IActionResult AddVendor() => Ok();

        [HttpGet]
        public IActionResult GetVendors() => Ok();

        [HttpPut("{id}")]
        public IActionResult UpdateVendor(int id) => Ok();

        [HttpDelete("{id}")]
        public IActionResult DeleteVendor(int id) => Ok();
    }
}
