using IMDBClone.API.Dtos.ReviewDtos;
using IMDBClone.API.Models;
using IMDBClone.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IMDBClone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly ReviewService _reviewService;

        public ReviewsController(ReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet]
        public async Task<IActionResult> GetReviews(string movieId)
        {
            var reviews = await _reviewService.GetByMovieIdAsync(movieId);
            return Ok(reviews);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddReview(string movieId, ReviewCreateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var review = new Review
            {
                UserId = userId ?? "anonymous",
                MovieId = movieId,
                Comment = dto.Comment,
                Rating = dto.Rating,
                CreatedAt = DateTime.UtcNow
            };

            await _reviewService.CreateAsync(review);
            return Ok(review);
        }
    }
}
