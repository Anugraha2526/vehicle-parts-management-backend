using Microsoft.AspNetCore.Mvc;
using VehicleParts.Application.Modules.Finance.DTOs;

namespace VehicleParts.Api.Controllers.Finance;

[ApiController]
[Route("api/[controller]")]
public sealed class PurchasesController : ControllerBase
{
    // TODO(Member 2): implement purchase invoice flow for stock updates.
    [HttpPost("invoice")]
    public IActionResult CreatePurchaseInvoice([FromBody] CreatePurchaseInvoiceDto request)
    {
        return StatusCode(StatusCodes.Status501NotImplemented,
            "Architecture phase only. Member 2 Finance implementation moved to Member2_Finance_Backup.");
    }
}
