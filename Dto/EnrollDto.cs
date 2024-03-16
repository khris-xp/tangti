namespace tangti.DTOs
{
    public class EnrollDto
    {
        public required string eventId { get; set; }
        public required string userId { get; set; }
    }

    public class UpdateStatusDto
    {
        public required string eventId { get; set; }
        public required string userId { get; set; }
        public required bool status { get; set; }
    }

    public class GetenrollMembersDto
    {
        public required string eventId { get; set; }
        public required bool status { get; set; }
    }

    public class KickEnrollMemberDto
    {
        public required string eventId { get; set; }
        public required string userId { get; set; } 
    } 
}