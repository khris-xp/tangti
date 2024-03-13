using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using tangti.Services;
using tangti.Models;

namespace MemberListController
{
    [Route("api/memberlist")]
    [ApiController]
    public class MemberListController : ControllerBase
    {
        private readonly EnrollService _enrollService;
        private readonly EventService _eventService;
        private readonly UserService _userService;

        public MemberListController(EnrollService enrollService, EventService eventService, UserService userService)
        {
            _enrollService = enrollService;
            _eventService = eventService;
            _userService = userService;
        }

        [HttpGet("{eventid:length(24)}")]
        public async Task<ActionResult> Get(string eventId)
        {
            var enrolls = await _enrollService.GetEventEnrollAsync(eventId);

            if (enrolls is null)
            {
                return BadRequest("Enroll is null");
            }

            var users = new List<UserModel>();

            foreach (var member in enrolls.MemberList)
            {
                if (member.enroll_status == true)
                {
                    var user = await _userService.GetUserAsync(member.UserID);
                    if (user is not null)
                    {
                        users.Add(user);
                    }
                }
            }

            var events = await _eventService.GetAsync(enrolls.EventID);

            if (events is null)
            {
                return BadRequest("Event is null");
            }

            var response_data = new
            {
                limit = events.EnrollLimit,
                num_enrolls = enrolls.Member,
                users = users
            };

            return Ok(response_data);
        }
    }
}