using tangti.Models;
using tangti.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Microsoft.VisualBasic;
using Microsoft.AspNetCore.Routing.Tree;
using tangti.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;

namespace EnrollController
{
    [Route("api/enroll")]
    [ApiController]
    public class EnrollController : ControllerBase
    {
        private readonly EnrollService _enrollService;
        private readonly UserService _userService;
        private readonly EventService _eventService;

        public EnrollController(EnrollService enrollService, UserService userService, EventService eventService)
        {
            _enrollService = enrollService;
            _userService = userService;
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Enroll>>> Get()
        {
            var enrolls = await _enrollService.GetAsync(); 

            return Ok(enrolls);
        }

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Enroll>> Get(string id)
        {
            var _enroll = await _enrollService.GetAsync(id);

            if (_enroll is null)
            {
                return NotFound();
            }

            return Ok(_enroll);
        }

        [HttpPost("create")]
        public async Task<ActionResult> Create(Enroll enroll)
        {
            try
            {
                string message_response;
                if (ModelState.IsValid)
                {
                    enroll.Id = ObjectId.GenerateNewId().ToString();
                    await _enrollService.CreateAsync(enroll);
                    message_response = "Enroll created Successfully";
                    //return some Data
                    return Ok(message_response);
                }
                else
                {
                    message_response = "invalid model state";
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Create action: {ex}");
                return NotFound();
            }
        }

        [HttpPost("edit")]
        public async Task<IActionResult> Edit(string id, Enroll enrollIn)
        {
            var enroll = await _enrollService.GetAsync(id);

            if (enroll is null)
            {
                return NotFound();
            }

            await _enrollService.UpdateAsync(id, enrollIn);

            return Ok(enroll);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            var enroll = await _enrollService.GetAsync(id);

            if (enroll == null)
            {
                return NotFound();
            }

            await _enrollService.DeleteAsync(id);

            return Ok(enroll);
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] EnrollDto enrollDto)
        {

            var enroll = await _enrollService.GetEventEnrollAsync(enrollDto.eventId);


            if (enroll is null)
            {
                return BadRequest();
            }

            if (enroll.MemberList.Any(member => member.UserID == enrollDto.userId))
            {
                // Can not join this Event because already joined
                return Ok(enroll);
            }

            var user = await _userService.GetUserAsync(enrollDto.userId);

            if (user is null)
            {
                return BadRequest();
            }

            user.Enrolled.Add(enroll.EventID);

            var _event = await _eventService.GetAsync(enrollDto.eventId);

            if( _event is null)
            {
                return BadRequest();
            }

            bool status = enroll.Member < _event.EnrollLimit;

            enroll.MemberList.Add(
                new Enroll.JoinUserData(enrollDto.userId, status)
            );

            enroll.Member = enroll.MemberList.Count;

            await _enrollService.UpdateAsync(enroll.Id, enroll);

            await _userService.UpdateUserAsync(enrollDto.userId, user);

            return Ok(enroll);
        }

        [HttpPost("unenroll")]
        public async Task<IActionResult> Unenroll([FromBody] EnrollDto enrollDto)
        {
            var enroll = await _enrollService.GetEventEnrollAsync(enrollDto.eventId);

            if (enroll is null)
            {
                return BadRequest();
            }

            if (!enroll.MemberList.Any(member => member.UserID == enrollDto.userId))
            {
                // Can not exit group this Event because not in this Event
                return Ok("Not in Event");
            }

            var user = await _userService.GetUserAsync(enrollDto.userId);

            if (user is null)
            {
                return BadRequest();
            }

            user.Enrolled.Remove(enroll.EventID);

            Enroll.JoinUserData? target = enroll.MemberList.FirstOrDefault(member => member.UserID == enrollDto.userId);

            if (target != null)
            {
                enroll.MemberList.Remove(target);
            }

            enroll.Member = enroll.MemberList.Count; ;

            await _enrollService.UpdateAsync(enroll.Id, enroll);

            await _userService.UpdateUserAsync(enrollDto.eventId, user);

            return Ok(enroll);
        }
    }
}