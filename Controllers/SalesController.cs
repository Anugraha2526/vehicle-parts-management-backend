using Microsoft.AspNetCore.Mvc;

namespace vehicle_parts_management_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        [HttpPost("invoice")]
        public IActionResult CreateSalesInvoice() => Ok();

        [HttpPost("invoice/{id}/email")]
        public IActionResult SendInvoiceEmail(int id) => Ok();
    }
}
