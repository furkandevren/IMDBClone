using IMDBClone.API.Dtos.ReviewDtos;

namespace IMDBClone.API.Dtos.MovieDtos
{
    public class MovieWithReviewsDto
    {
        public string? Id { get; set; }
        public string Title { get; set; } = null!;
        public List<string> Genres { get; set; } = new();
        public int Year { get; set; }
        public string Overview { get; set; } = null!;
        public string? PosterUrl { get; set; }
        public double Rating { get; set; }
        public int RatingCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ReviewReadDto> Reviews { get; set; } = new();
    }
}
