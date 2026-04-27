using Microsoft.AspNetCore.Mvc;
using VehicleParts.Application.Modules.CustomerPortal.Interfaces;

namespace VehicleParts.Api.Controllers.CustomerPortal;

[ApiController]
[Route("api/[controller]")]
public sealed class ReviewsController : ControllerBase
{
    private readonly ICustomerPortalService _customerPortalService;

    public ReviewsController(ICustomerPortalService customerPortalService)
    {
        _customerPortalService = customerPortalService;
    }

    [HttpPost]
    public async Task<IActionResult> SubmitReview(CancellationToken cancellationToken)
    {
        var result = await _customerPortalService.SubmitReviewAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetReviews(CancellationToken cancellationToken)
    {
        var result = await _customerPortalService.GetReviewsAsync(cancellationToken);
        return Ok(result);
    }
}
