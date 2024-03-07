namespace tangti.DTOs
{
    public class RegisterDto
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Phone { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string ImageProfile { get; set; }
        public required string[] Enrolled { get; set; }
        public required string[] EventCreated { get; set; }
    }

    public class LoginDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}