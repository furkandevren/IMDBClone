namespace IMDBClone.API.Dtos
{
    public class MovieReadDto
    {
        public string Id { get; set; } = null!;
        public string Title { get; set; } = null!;
        public int Year { get; set; }
        public string Overview { get; set; } = null!;
        public List<string> Genres { get; set; } = new();
        public string PosterUrl { get; set; } = null!;
        public double Rating { get; set; }
        public int RatingCount { get; set; }
    }
}
