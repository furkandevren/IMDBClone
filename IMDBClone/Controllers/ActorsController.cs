using IMDBClone.API.Dtos.ActorDtos;
using IMDBClone.API.Models;
using IMDBClone.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace IMDBClone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActorsController : ControllerBase
    {
        private readonly ActorService _actorService;

        public ActorsController(ActorService actorService)
        {
            _actorService = actorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var actors = await _actorService.GetAllAsync();
            var dto = actors.Select(a => new ActorReadDto
            {
                Id = a.Id!,
                Name = a.Name,
                Bio = a.Bio,
                BirthDate = a.BirthDate,
                PhotoUrl = a.PhotoUrl,
                KnownForMovieIds = a.KnownForMovieIds
            }).ToList();

            return Ok(dto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var actor = await _actorService.GetByIdAsync(id);
            if (actor == null) return NotFound();
            var dto = new ActorReadDto
            {
                Id = actor.Id!,
                Name = actor.Name,
                Bio = actor.Bio,
                BirthDate = actor.BirthDate,
                PhotoUrl = actor.PhotoUrl,
                KnownForMovieIds = actor.KnownForMovieIds
            };
            return Ok(dto);
        }

        [HttpGet("{id}/movies")]
        public async Task<IActionResult> GetMovies(string id)
        {
            var movies = await _actorService.GetMoviesByActorIdAsync(id);
            return Ok(movies); // istersen burada MovieReadDto mapleyebilirsin
        }

        [HttpGet("top")]
        public async Task<IActionResult> GetTop([FromQuery] int limit = 10)
        {
            var top = await _actorService.GetTopActorsAsync(limit);
            return Ok(top);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ActorCreateDto dto)
        {
            var actor = new Actor
            {
                Name = dto.Name,
                Bio = dto.Bio,
                BirthDate = dto.BirthDate,
                PhotoUrl = dto.PhotoUrl
            };
            await _actorService.CreateAsync(actor);
            return CreatedAtAction(nameof(Get), new { id = actor.Id }, actor);
        }
    }
}