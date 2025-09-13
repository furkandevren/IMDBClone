namespace IMDBClone.API.Dtos.ReviewDtos
{
    public class ReviewDto
    {
        public string Id { get; set; }=string.Empty;
        public string UserId { get; set; }=string.Empty;
        public string Comment { get; set; }= string.Empty;
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
