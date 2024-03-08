using tangti.Models;

namespace tangti.DTOs
{
    public class CommentDto
    {
        public required string eventId { get; set; }

        public required string content { get; set; }

        public required string userId { get; set; }

        public required string id { get; set; }
    }
}