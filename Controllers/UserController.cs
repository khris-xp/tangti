using Microsoft.AspNetCore.Mvc;
using tangti.DTOs;
using tangti.Services;

namespace UserController
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly UserService _userService;

        public AccountController(AuthService authService, UserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterDto user)
        {
            var user_response = await _authService.Register(user);
            return Ok(user_response);
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginDto user)
        {
            var user_response = await _authService.Login(user.Email, user.Password);
            return Ok(user_response);
        }

        [HttpGet("user")]
        public async Task<ActionResult> GetUser([FromBody] string userId)
        {
            var user_response = await _userService.GetUserAsync(userId);
            Console.WriteLine(user_response);
            return Ok(user_response);
        }
    }
}
