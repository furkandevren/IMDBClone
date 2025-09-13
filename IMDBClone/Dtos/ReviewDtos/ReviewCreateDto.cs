namespace IMDBClone.API.Dtos.ReviewDtos
{
    public class ReviewCreateDto
    {
        public string Comment { get; set; } =string.Empty;
        public int Rating { get; set; }
    }
}
