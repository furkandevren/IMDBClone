using IMDBClone.API.Dtos;
using IMDBClone.API.Dtos.ReviewDtos;
using IMDBClone.API.Models;
using IMDBClone.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IMDBClone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly ReviewService _reviewService;

        public ReviewsController(ReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("movie/{movieId}")]
        public async Task<IActionResult> GetByMovieId(string movieId)
        {
            var reviews = await _reviewService.GetByMovieIdAsync(movieId);
            var dtoList = reviews.Select(r => new ReviewReadDto
            {
                Id = r.Id,
                Content = r.Content,
                Rating = r.Rating,
                UserId = r.UserId,
                MovieId = r.MovieId,
                CreatedAt = r.CreatedAt
            }).ToList();

            return Ok(dtoList);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var review = await _reviewService.GetByIdAsync(id);
            if (review == null) return NotFound();

            return Ok(new ReviewReadDto
            {
                Id = review.Id,
                Content = review.Content,
                Rating = review.Rating,
                UserId = review.UserId,
                MovieId = review.MovieId,
                CreatedAt = review.CreatedAt
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReviewCreateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var review = await _reviewService.CreateAsync(dto, userId);

            return CreatedAtAction(nameof(GetById), new { id = review.Id }, review);
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ReviewUpdateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var success = await _reviewService.UpdateAsync(dto, userId);

            if (!success) return Unauthorized();

            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var success = await _reviewService.DeleteAsync(id, userId);

            if (!success) return Unauthorized();

            return NoContent();
        }
    }
}
