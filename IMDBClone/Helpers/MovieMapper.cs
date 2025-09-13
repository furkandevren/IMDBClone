using IMDBClone.API.Dtos;
using IMDBClone.API.Models;

namespace IMDBClone.API.Helpers
{
    public static class MovieMapper
    {
        public static MovieReadDto ToReadDto(this Movie m) =>
            new MovieReadDto
            {
                Id = m.Id!,
                Title = m.Title,
                Year = m.Year,
                Overview = m.Overview,
                Genres = m.Genres,
                PosterUrl = m.PosterUrl,
                Rating = m.Rating,
                RatingCount = m.RatingCount
            };

        public static Movie FromCreateDto(this MovieCreateDto dto) =>
            new Movie
            {
                Title = dto.Title,
                Year = dto.Year,
                Overview = dto.Overview,
                Genres = dto.Genres,
                PosterUrl=dto.PosterUrl,
                Rating=0,
                RatingCount=0,
                CreatedAt = DateTime.UtcNow

            };

        public static Movie FromUpdateDto(this MovieUpdateDto dto, Movie existing)
        {
            // Sadece güncellenmesini istediğimiz alanları değiştirelim
            existing.Title = dto.Title;
            existing.Year = dto.Year;
            existing.Overview = dto.Overview;
            existing.Genres = dto.Genres ?? new List<string>();
            existing.PosterUrl = dto.PosterUrl;

            // Id, Rating, RatingCount, CreatedAt gibi alanları koruyoruz
            return existing;
        }
    }
}
