namespace tangti.DTOs
{
    public class CreateLikeDto
    {
        public required string eventId { get; set;}
    }

    public class LikeDto
    {
        public required string userId { get; set; }
        public required string eventId { get; set; }
    }
}