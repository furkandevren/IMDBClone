namespace IMDBClone.API.Dtos.MovieDtos
{
    public class CastMemberCreateDto
    {
        public string ActorId { get; set; } = null!;
        public string Character { get; set; }
        public int Order { get; set; } = 0;
    }
}
