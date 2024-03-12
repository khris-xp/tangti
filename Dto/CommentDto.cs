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

    public class DeleteCommentDto
    {
        public required string id { get; set; }
    }

    public class UpdateCommentDto
    {
        public required string id { get; set; }
        public required string content { get; set; }
    }

    public class EventCommentDto
    {
        public required string eventId { get; set; }
    }
}