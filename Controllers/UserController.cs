using Microsoft.AspNetCore.Mvc;
using tangti.DTOs;
using tangti.Services;

namespace UserController
{
    [Route("api/user")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AuthService _authService;

        public AccountController(AuthService authService)
        {
            _authService = authService;
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
    }
}
