using Microsoft.AspNetCore.Mvc;
using tangti.DTOs;
using tangti.Services;
using tangti.Models;

namespace UserController
{
    [Route("api/user")]
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

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<UserModel>> GetUser(string id)
        {
            var user = await _userService.GetUserAsync(id);

            if (user is null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpGet("event")]
        public async Task<ActionResult<List<Event>>> GetEvents([FromBody] string id)
        {
            var events = await _userService.GetUserEventsAsync(id);
            if (events is null)
            {
                return BadRequest("No events found.");
            }
            return Ok(events);
        }

        [HttpPost("update")]
        public async Task<ActionResult> Update([FromBody] UpdateDto updateDto)
        {
            var user = await _userService.GetUserAsync(updateDto.userId);

            if(user is null)
            {
                return BadRequest("No User found.");
            }

            user.FirstName = updateDto.FirstName;
            user.LastName = updateDto.LastName;
            user.Email = updateDto.Email;
            user.Phone = updateDto.TelephoneNumber;
            user.FullName = user.FirstName + " " + user.LastName;

            if (user.Id is null)
            {
                return BadRequest("User ID is null.");
            }

            await _userService.UpdateUserAsync(user.Id, user);

            return Ok(user);
        }
    }
}
