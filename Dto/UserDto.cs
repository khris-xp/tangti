using tangti.Models;

namespace tangti.DTOs
{
    public class RegisterDto
    {
        public required string Username { get; set; }

        public required string Email { get; set; }

        public required string Password { get; set; }

        public static implicit operator RegisterDto(UserModel v)
        {
            throw new NotImplementedException();
        }
    }

    public class LoginDto
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }

    public class UserDto
    {
        public required string Username { get; set; }
        public required string Token { get; set; }
    }
}