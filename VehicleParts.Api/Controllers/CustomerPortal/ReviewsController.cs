using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VehicleParts.Application.Modules.CustomerPortal.DTOs;
using VehicleParts.Application.Modules.CustomerPortal.Interfaces;

namespace VehicleParts.Api.Controllers.CustomerPortal;

[ApiController]
[Route("api/[controller]")]
public sealed class ReviewsController : ControllerBase
{
    private readonly ICustomerPortalService _portalService;

    public ReviewsController(ICustomerPortalService portalService)
    {
        _portalService = portalService;
    }

    // GET /api/Reviews — public; all users (including anonymous) can read reviews
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetReviews(CancellationToken cancellationToken)
    {
        var result = await _portalService.GetReviewsAsync(cancellationToken);
        return Ok(result);
    }

    // POST /api/Reviews — only authenticated customers can submit a review
    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> SubmitReview(
        [FromBody] SubmitReviewDto request,
        CancellationToken cancellationToken)
    {
        var customerId = GetCustomerId();
        if (customerId == Guid.Empty) return Unauthorized();

        var result = await _portalService.SubmitReviewAsync(customerId, request, cancellationToken);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    private Guid GetCustomerId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(raw, out var id) ? id : Guid.Empty;
    }
}
