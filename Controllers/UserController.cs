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
        private readonly TokenService _tokenService;

        public AccountController(AuthService authService, TokenService tokenService)
        {
            _authService = authService;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterDto user)
        {
            var user_response = await _authService.Register(user.Username, user.Email, user.Password);
            return Ok(user_response);
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginDto user)
        {
            await _authService.Login(user.Username, user.Password);
            return Ok();
        }
    }
}
