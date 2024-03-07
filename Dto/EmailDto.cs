namespace tangti.DTOs
{
    public class EmailDto
    {
        public required string ToAddress { get; set; }
        public required string Subject { get; set; }
        public required string Body { get; set; }
    }

}