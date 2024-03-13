using tangti.Models;
using tangti.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver.Core.Operations;

namespace tangti.Controllers;

public class EventController : Controller
{
    private readonly EventService _eventsService;
    private readonly EnrollService _enrollService;
    private readonly CategoryService _categoryService;
    private readonly ReportService _reportService;
    public EventController(EventService eventsService, EnrollService enrollService, CategoryService categoryService, ReportService reportService, UserService userService)
    {
        _eventsService = eventsService;
        _categoryService = categoryService;
        _reportService = reportService;
        _enrollService = enrollService;
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
            if (curr_event.Id != null)
            {
                enroll_inst = await _enrollService.GetEventEnrollAsync(curr_event.Id);
				// change status
				_eventsService.checkStatus(curr_event);
				Console.WriteLine("Event:" + curr_event.Title + " type:" + curr_event.Type + " status: " + curr_event.Status);
            	if (await _eventsService.isTimeClose(curr_event.Id))
             	   Console.WriteLine(curr_event.Title + ": Notifination (by time)");
            	if (enroll_inst != null && await _eventsService.isTouchLimit(curr_event.Id, enroll_inst))
				{
             	   Console.WriteLine(curr_event.Title + ": Notifination (by members limit)");
				   if (curr_event.Type != "Queue" && curr_event.Status != "CLOSED")
				   {
					   _eventsService.changeStatus(curr_event.Id, "CLOSED");
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
                    ViewBag.Message = UtilsService.ValidateErrorTime(events.EventDate, events.EnrollDate);
                    return View();
                }
                else
                {
                    Console.WriteLine("Created By : ", events.CreatedBy);
                    events.Id = ObjectId.GenerateNewId().ToString();
                    await _eventsService.CreateAsync(events);
                    Enroll newEnroll = new Enroll
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
        Console.WriteLine("Report: " + report.Description);
        report.Id = ObjectId.GenerateNewId().ToString();
        report.EventId = id;

        await _reportService.CreateAsync(report);

        return RedirectToAction("Details", "Event", new { id = id });
    }
}
