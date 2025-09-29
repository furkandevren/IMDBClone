namespace IMDBClone.API.Dtos.ActorDtos
{
    public class ActorCreateDto
    {
        public string Name { get; set; } = null!;
        public string? Bio { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? PhotoUrl { get; set; }
    }
}
