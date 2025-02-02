using tangti.Models;
using tangti.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace tangti.Controllers;

public class EventController : Controller
{
    private readonly EventService _eventsService;

    private readonly EmailService _emailService;

    private readonly EnrollService _enrollService;
    private readonly CategoryService _categoryService;
    private readonly ReportService _reportService;

    private readonly UserService _userService;
    private readonly LikeService _likeService;
    public EventController(EventService eventsService, EnrollService enrollService, CategoryService categoryService, ReportService reportService, UserService userService,EmailService emailService, LikeService likeService)
    {
        _eventsService = eventsService;
        _categoryService = categoryService;
        _reportService = reportService;
        _enrollService = enrollService;
        _emailService = emailService;
        _userService = userService;
        _likeService = likeService;
    }

    public async Task<IActionResult> Index(string searchString, string category, int page = 1, int pageSize = 6)
    {
        var events = await _eventsService.GetPaganationAsync(page, pageSize, searchString, category);

        var categories_list = await _categoryService.GetCategoryNamesAsync();

        ViewBag.Category = category;
        ViewBag.Categories_list = categories_list;
        ViewBag.SearchString = searchString;
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalCount = await _eventsService.GetTotalCountAsync(searchString); // Assuming you have a method to get total count

        foreach (var curr_event in events)
        {
            Enroll? enroll_inst = null;
            if (curr_event.Id != null && curr_event.Status != "CLOSED")
            {
                enroll_inst = await _enrollService.GetEventEnrollAsync(curr_event.Id);
                await _eventsService.checkStatus(curr_event);
                if (await _eventsService.isTimeClose(curr_event.Id))
                {
                    await _eventsService.changeStatus(curr_event.Id, "CLOSED");

                    var members = enroll_inst.MemberList;
                        foreach (var member in members)
                        {
                            var user = await _userService.GetUserAsync(member.UserID);
                            if (user != null)
                            {                
                                string subject = "Tangti: Your Event has been closed";
                                string body = "<h1>Your event has been closed.</h1> \n <h2>Event: "+ curr_event.Title + "</h2> \n<img src ='"+ curr_event.Image +"'><br> \n You can see member in https://kmitltangti.azurewebsites.net/Event/memberlist/" + enroll_inst.EventID;
                                await _emailService.SendEmail(user.Email, subject, body);
                            }
                        }
                }
                if (enroll_inst != null && await _eventsService.isTouchLimit(curr_event.Id, enroll_inst))
                {
                    if (curr_event.Type != "Queue" && curr_event.Status != "CLOSED")
                    {
                        await _eventsService.changeStatus(curr_event.Id, "CLOSED");

                        var members = enroll_inst.MemberList;
                        foreach (var member in members)
                        {
                            var user = await _userService.GetUserAsync(member.UserID);
                            if (user != null)
                            {
                                string subject = "Tangti: Your Event has been closed";
                                string body = "<h1>Your event has been closed.</h1> \n <h2>Event: "+ curr_event.Title + "</h2> \n<img src ='"+ curr_event.Image +"'><br> \n You can see member in https://kmitltangti.azurewebsites.net/Event/memberlist/" + enroll_inst.EventID;
                                await _emailService.SendEmail(user.Email, subject, body);
                            }
                        }
                    }
                }
                if (enroll_inst != null)
                {
                    curr_event.members = enroll_inst.Member;
                }
            }
        }

        return View(events);
    }

    public IActionResult Details(string id)
    {
        var events = _eventsService.GetAsync(id).Result;
        return View(events);
    }
    public async Task<IActionResult> Create()
    {
        var categories = await _categoryService.GetCategoryNamesAsync();
        ViewBag.Categories = categories;
        return View();
    }
    public async Task<IActionResult> Edit(string id)
    {
        var events = await _eventsService.GetAsync(id);

        var categories = await _categoryService.GetCategoryNamesAsync();

        ViewBag.Categories = categories;
        ViewBag.DefaultCategory = events.Category;
        ViewBag.DefaultType = events.Type;

        return View(events);
    }

    public ActionResult Delete(string id)
    {
        var events = _eventsService.GetAsync(id).Result;
        return View(events);
    }

    public ActionResult Report(string id)
    {
        Console.WriteLine(id);
        var reports = _reportService.GetReportAsync(id).Result;
        return View(reports);
    }

    public ActionResult Memberlist(string id)
    {
        var events = _eventsService.GetAsync(id).Result;
        return View(events);
    }

    [HttpGet]
    public async Task<ActionResult<List<Event>>> Get()
    {
        return await _eventsService.GetAsync();
    }

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Event>> Get(string id)
    {
        var _event = await _eventsService.GetAsync(id);

        if (_event is null)
        {
            return NotFound();
        }

        return _event;
    }
    [HttpPost]
    public async Task<ActionResult> Create(Event events)
    {
        try
        {
            string message_response;
            if (ModelState.IsValid)
            {
                if (UtilsService.ValidateErrorTime(events.EventDate, events.EnrollDate) != "")
                {
                    ViewBag.Categories = await _categoryService.GetCategoryNamesAsync();
                    ViewBag.Message = UtilsService.ValidateErrorTime(events.EventDate, events.EnrollDate);
                    return View();
                }
                else
                {
                    events.Id = ObjectId.GenerateNewId().ToString();
                    await _eventsService.CreateAsync(events);
                    Enroll newEnroll = new()
                    {
                        EventID = events.Id,
                        Id = ObjectId.GenerateNewId().ToString(),
                        Member = 0
                    };
                    try
                    {
                        await _enrollService.CreateAsync(newEnroll);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }

                    Like newLike = new()
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        EventId = events.Id
                    };

                    await _likeService.CreateAsync(newLike);

                    message_response = "Event created successfully";
                    ViewBag.Message = message_response;
                    return RedirectToAction("Index");
                }
            }
            else
            {
                message_response = "Invalid model state";
                ViewBag.Message = message_response;
                return View();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Create action: {ex}");
            return View("Error");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(string id, Event updateEvent)
    {
        var events = await _eventsService.GetAsync(id);

        if (events is null)
        {
            return NotFound();
        }
        if (events.CreatedBy == null)
        {
            ViewBag.Message = "You are not authorized to edit this event";
            return View();
        }
        if (UtilsService.ValidateErrorTime(updateEvent.EventDate, updateEvent.EnrollDate) != "")
        {
            ViewBag.Message = UtilsService.ValidateErrorTime(updateEvent.EventDate, updateEvent.EnrollDate);
            return View();
        }
        await _eventsService.UpdateAsync(id, updateEvent);
        return RedirectToAction("Index");
    }
    [HttpPost]
    public async Task<IActionResult> DeleteAsync(string id)
    {
        var _event = await _eventsService.GetAsync(id);

        if (_event == null)
        {
            return NotFound();
        }

        await _eventsService.DeleteAsync(id);

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> ChangeStatus(string id, string new_status)
    {
        var events = await _eventsService.GetAsync(id);

        if (new_status != "NOT_OPENED" || new_status != "ON_GOING" || new_status != "CLOSED" || new_status != "CANCELED" || new_status != "BANNED")
        {
            return NotFound();
        }
        if (events is null)
        {
            return NotFound();
        }
        events.Status = new_status;
        await _eventsService.UpdateAsync(id, events);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Report(string id, Report report)
    {
        var events = await _eventsService.GetAsync(id);

        if (events is null)
        {
            return BadRequest("Event is null");
        }

        Console.WriteLine("Report: " + report.Description);
        report.Id = ObjectId.GenerateNewId().ToString();
        report.EventId = id;
        report.EventName = events.Title;

        await _reportService.CreateAsync(report);

        return RedirectToAction("Details", "Event", new { id = id });
    }
}
