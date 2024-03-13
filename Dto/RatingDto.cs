namespace tangti.DTOs
{
    public class CreateRatingDto
    {
        public required string eventId { get; set;}
        public required string userId { get; set;}
        public required float ratingScore { get; set; }
    }

    public class UpdateRatingDto
    {
        public required string id { get; set; }
        public required float ratingScore { get; set;}
    }

    public class DeleteRatingDto
    {
        public required string id { get; set; }
    }
}