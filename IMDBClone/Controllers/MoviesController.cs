using IMDBClone.API.Dtos;
using IMDBClone.API.Helpers;
using IMDBClone.API.Models;
using IMDBClone.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? genre = null)
        {
            var movies = await _movieService.GetAllAsync(page, pageSize, genre);
            var movieDtos = movies.Select(m => m.ToReadDto()).ToList();
            return Ok(movieDtos);

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var movie = await _movieService.GetByIdAsync(id);
            if (movie == null)
            {
                return NotFound(new { message = "Movie not found" });
            }

            return Ok(movie.ToReadDto());
        }

        [HttpPost]
        [Authorize] // sadece authenticated kullanıcılar ekleyebilir
        public async Task<IActionResult> Create(MovieCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var movie = dto.FromCreateDto();
            await _movieService.CreateAsync(movie);

            var result = movie.ToReadDto();

            return CreatedAtAction(nameof(GetById), new { id = movie.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(string id, MovieUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _movieService.GetByIdAsync(id);
            if (existing == null) return NotFound();

            // DTO → Entity (update işlemi için)
            var updated = dto.FromUpdateDto(existing);

            await _movieService.UpdateAsync(id, updated);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]

        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _movieService.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = "Movie not found" });

            await _movieService.DeleteAsync(id);
            return NoContent();
        }


    }
}
