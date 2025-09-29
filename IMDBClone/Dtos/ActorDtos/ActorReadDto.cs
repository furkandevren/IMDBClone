namespace IMDBClone.API.Dtos.ActorDtos
{
    public class ActorReadDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Bio { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? PhotoUrl { get; set; }
        public List<string> KnownForMovieIds { get; set; } = new();
    }
}
