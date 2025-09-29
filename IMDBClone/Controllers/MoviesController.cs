using IMDBClone.API.Dtos.MovieDtos;
using IMDBClone.API.Helpers;
using IMDBClone.API.Models;
using IMDBClone.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMDBClone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly MovieService _movieService;
        private readonly ILogger<MoviesController> _logger;

        public MoviesController(MovieService movieService, ILogger<MoviesController> logger)
        {
            _movieService = movieService;
            _logger = logger;
        }

        // 1. Herkese açık listeleme
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? genre = null)
        {
            var movies = await _movieService.GetAllAsync(page, pageSize, genre);
            var movieDtos = movies.Select(m => m.ToReadDto()).ToList();
            return Ok(movieDtos);
        }

        // 2. Herkese açık tekil film getir
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(string id)
        {
            var movie = await _movieService.GetByIdAsync(id);
            if (movie == null)
                return NotFound(new { message = "Movie not found" });

            return Ok(movie.ToReadDto());
        }

        // 3. Film ekleme - sadece Admin
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] MovieCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var movie = dto.FromCreateDto();
            await _movieService.CreateAsync(movie);

            var result = movie.ToReadDto();
            return CreatedAtAction(nameof(GetById), new { id = movie.Id }, result);
        }

        // 4. Film güncelleme - sadece Admin
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(string id, [FromBody] MovieUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _movieService.GetByIdAsync(id);
            if (existing == null) return NotFound();

            var updated = dto.FromUpdateDto(existing);
            await _movieService.UpdateAsync(id, updated);

            return NoContent();
        }

        // 5. Film silme - sadece Admin
        [HttpDelete("{id}")] 
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _movieService.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = "Movie not found" });

            await _movieService.DeleteAsync(id);
            return NoContent();
        }


        [HttpGet("{id}/with-reviews")]
        public async Task<IActionResult> GetWithReviews(string id, [FromServices] ReviewService reviewService)
        {
            var movie = await _movieService.GetWithReviewsAsync(id, reviewService);
            if (movie == null) return NotFound();

            return Ok(movie);
        }

    }
}