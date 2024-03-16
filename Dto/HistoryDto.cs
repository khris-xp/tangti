namespace tangti.DTOs
{
    public class HistoryDto
    {
        public required string eventId { get; set; }
        public required string eventName { get; set; }

        public required string userId { get; set; }

        public required string content { get; set; }
        
    }

    public class DeleteHistoryDto
    {
        public required string eventId { get; set; }

        public required string userId { get; set; }
    }

}