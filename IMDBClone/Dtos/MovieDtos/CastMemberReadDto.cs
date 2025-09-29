namespace IMDBClone.API.Dtos.MovieDtos
{
    public class CastMemberReadDto
    {
        public string ActorId { get; set; } = null!;
        public string? Character { get; set; }
        public int Order { get; set; }
    }
}
