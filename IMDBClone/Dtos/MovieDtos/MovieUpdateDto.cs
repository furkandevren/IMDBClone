namespace IMDBClone.API.Dtos.MovieDtos
{
    public class MovieUpdateDto
    {
        public string Title { get; set; } = null!;
        public int Year { get; set; }
        public string Overview { get; set; } = null!;
        public List<string> Genres { get; set; } = new();
        public string PosterUrl { get; set; } = null!;
        public DateTime ReleaseDate { get; set; }
        public List<CastMemberCreateDto> Cast { get; set; } = new();
    }
}
