namespace IMDBClone.API.Dtos.ReviewDtos
{
    public class ReviewReadDto
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public int Rating { get; set; }
        public string UserId { get; set; }
        public string MovieId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
