using tangti.Models;
using tangti.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using tangti.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;

namespace EnrollController
{
    [Route("api/enroll")]
    [ApiController]
    public class EnrollController : ControllerBase
    {
        private readonly EnrollService _enrollService;
        private readonly UserService _userService;
        private readonly EventService _eventService;

        private readonly EmailService _emailService;

        // add logger
        private readonly ILogger<EnrollController> _logger;

        public EnrollController(EnrollService enrollService, UserService userService, EventService eventService, EmailService emailService, ILogger<EnrollController> logger)
        {
            _enrollService = enrollService;
            _userService = userService;
            _eventService = eventService;
            _emailService = emailService;
            _logger = logger;
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
                return BadRequest("Enroll is null");
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
                    return BadRequest(message_response);
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
                return BadRequest("Enroll is null");
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
                return BadRequest("Enroll is null");
            }

            await _enrollService.DeleteAsync(id);

            return Ok(enroll);
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] EnrollDto enrollDto)
        {
            if (enrollDto.userId is null)
            {
                return BadRequest("User Id is null");
            }

            var enroll = await _enrollService.GetEventEnrollAsync(enrollDto.eventId);

            if (enroll is null)
            {
                return BadRequest("Enroll is null");
            }


            if (enroll.MemberList.Any(member => member.UserID == enrollDto.userId))
            {
                // Can not join this Event because already joined
                return Ok(enroll);
            }

            var user = await _userService.GetUserAsync(enrollDto.userId);

            if (user is null)
            {
                return BadRequest("User is null");
            }

            user.Enrolled.Add(enroll.EventID);

            var _event = await _eventService.GetAsync(enrollDto.eventId);

            if (_event is null)
            {
                return BadRequest("Event is null");
            }

            bool status = enroll.Member < _event.EnrollLimit;

            enroll.MemberList.Add(
                new Enroll.JoinUserData(enrollDto.userId, status)
            );




            enroll.Member = enroll.MemberList.Count;

            if (enroll.Id == null)
            {
                return BadRequest("Enroll Id is null");
            }

            await _enrollService.UpdateAsync(enroll.Id, enroll);


            //Email Here
            // _logger.LogInformation("Sending Email to " + user.Email);

            // string subject = "Enroll Success";
            // string body = "You have successfully enrolled in " + _event.Title + "." +
            // "That will start on " + _event.EventDate.StartDate + "to " + _event.EventDate.EndDate + ".";

            // await _emailService.SendEmail(user.Email, subject, body);



            await _userService.UpdateUserAsync(enrollDto.userId, user);

            return Ok(enroll);
        }

        [HttpPost("unenroll")]
        public async Task<IActionResult> Unenroll([FromBody] EnrollDto enrollDto)
        {
            if (enrollDto.userId is null)
            {
                return BadRequest("User Id is null");
            }


            var enroll = await _enrollService.GetEventEnrollAsync(enrollDto.eventId);

            if (enroll is null)
            {
                return BadRequest("Enroll is null");
            }

            if (!enroll.MemberList.Any(member => member.UserID == enrollDto.userId))
            {
                // Can not exit group this Event because not in this Event
                return Ok("Not in Event");
            }

            var user = await _userService.GetUserAsync(enrollDto.userId);

            if (user is null)
            {
                return BadRequest("User is null");
            }

            user.Enrolled.Remove(enroll.EventID);

            Enroll.JoinUserData? target = enroll.MemberList.FirstOrDefault(member => member.UserID == enrollDto.userId);

            if (target != null)
            {
                enroll.MemberList.Remove(target);
            }

            enroll.Member = enroll.MemberList.Count; ;

            if (enroll.Id == null)
            {
                return BadRequest("Enroll Id is null");
            }
            await _enrollService.UpdateAsync(enroll.Id, enroll);


            //Send Email
            // _logger.LogInformation("Sending Email to " + user.Email);

            // string subject = "Unenroll Success";
            // string body = "You have successfully unenrolled in " + enroll.EventID + ".";
            // await _emailService.SendEmail(user.Email, subject, body);


            await _userService.UpdateUserAsync(enrollDto.userId, user);

            return Ok(enroll);
        }

        [HttpPost("check")]
        public async Task<IActionResult> Check([FromBody] EnrollDto enrollDto)
        {
            var enroll = await _enrollService.GetEventEnrollAsync(enrollDto.eventId);

            if (enroll is null)
            {
                return BadRequest("Enroll not Found");
            }

            bool Result = enroll.MemberList.Any(member => member.UserID == enrollDto.userId);

            // if enroll in event return true : if not enroll return false
            return Ok(Result);
        }

        [HttpPost("updatestatus")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusDto UpdateStatusDto)
        {
            var enroll = await _enrollService.GetEventEnrollAsync(UpdateStatusDto.eventId);

            if (enroll is null)
            {
                return BadRequest("Enroll not Found");
            }

            if (enroll.Id is null)
            {
                return BadRequest("Enroll id is null");
            }

            foreach (var member in enroll.MemberList)
            {
                if (member.UserID == UpdateStatusDto.userId)
                {
                    member.enroll_status = UpdateStatusDto.status;

                    await _enrollService.UpdateAsync(enroll.Id, enroll);

                    return Ok(member);
                }
            }

            return Ok("User Not in Enrollment");
        }

        [HttpPost("getmember")]
        public async Task<ActionResult> GetMember([FromBody] GetenrollMembersDto enrollDto)
        {
            var enroll = await _enrollService.GetEventEnrollAsync(enrollDto.eventId);

            if (enroll is null)
            {
                return BadRequest("Enroll is null");
            }

            var members = new List<Enroll.JoinUserData>();
            // Check status
            foreach (var member in enroll.MemberList)
            {
                if (member.enroll_status == enrollDto.status)
                {
                    members.Add(member);
                }
            }
            enroll.MemberList = members;

            enroll.Member = enroll.MemberList.Count;

            return Ok(enroll);
        }

        [HttpGet("event/{eventid:length(24)}")]
        public async Task<ActionResult> GetEnrollByEventId(string eventid)
        {
            var enroll = await _enrollService.GetEventEnrollAsync(eventid);

            if (enroll is null)
            {
                return BadRequest("Enroll is null");
            }

            return Ok(enroll);
        }

        [HttpPost("kick")]
        public async Task<ActionResult> KickMember([FromBody] KickEnrollMemberDto kickEnrollMemberDto)
        {
            var enroll = await _enrollService.GetEventEnrollAsync(kickEnrollMemberDto.eventId);

            if (enroll is null)
            {
                return BadRequest("Enroll is null");
            }

            foreach (var member in enroll.MemberList)
            {
                if (member.UserID == kickEnrollMemberDto.userId)
                {
                    enroll.MemberList.Remove(member);

                    enroll.Member = enroll.MemberList.Count;

                    if (enroll.Id != null)
                        await _enrollService.UpdateAsync(enroll.Id, enroll);

                    return Ok(enroll);
                }
            }

            return Ok(enroll);
        }
    }
}