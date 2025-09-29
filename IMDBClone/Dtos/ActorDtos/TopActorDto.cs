namespace IMDBClone.API.Dtos.ActorDtos
{
    public class TopActorDto
    {
        public string ActorId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? PhotoUrl { get; set; }
        public double AvgMovieRating { get; set; }
        public int MovieCount { get; set; }
    }
}
