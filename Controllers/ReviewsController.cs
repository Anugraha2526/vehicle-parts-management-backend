using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vehicle_parts_management_backend.Domain.Entities;
using vehicle_parts_management_backend.Infrastructure.Data;

namespace vehicle_parts_management_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Submit Review
        [HttpPost]
        public async Task<IActionResult> SubmitReview(Review review)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            review.CreatedAtUtc = DateTime.UtcNow;

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Review submitted successfully",
                reviewId = review.Id
            });
        }

        // ✅ Get All Reviews
        [HttpGet]
        public async Task<IActionResult> GetReviews()
        {
            var reviews = await _context.Reviews
                .OrderByDescending(r => r.CreatedAtUtc)
                .ToListAsync();

            return Ok(reviews);
        }

        // ✅ Update Review
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReview(int id, Review updated)
        {
            var review = await _context.Reviews.FindAsync(id);

            if (review == null)
                return NotFound();

            review.Rating = updated.Rating;
            review.Comment = updated.Comment;
            review.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(review);
        }

        // ✅ Delete Review
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);

            if (review == null)
                return NotFound();

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return Ok("Review deleted successfully");
        }
    }
}