using Microsoft.AspNetCore.Mvc;

namespace vehicle_parts_management_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        [HttpPost]
        public IActionResult SubmitReview() => Ok();

        [HttpGet]
        public IActionResult GetReviews() => Ok();
    }
}
