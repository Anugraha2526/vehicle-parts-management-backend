using Microsoft.AspNetCore.Mvc;

namespace vehicle_parts_management_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchasesController : ControllerBase
    {
        [HttpPost("invoice")]
        public IActionResult CreatePurchaseInvoice() => Ok();
    }
}
