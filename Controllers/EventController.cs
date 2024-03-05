using tangti.Models;
using tangti.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace tangti.Controllers;

public class EventController : Controller
{
    private readonly EventService _eventsService;
    private readonly EnrollService _enrollService;

    public EventController(EventService eventsService,EnrollService enrollService)
    {
        _eventsService = eventsService;
        _enrollService = enrollService;
    }

    public IActionResult Index()
    {
        var events = await _eventsService.GetPaganationAsync(page, pageSize, searchString);

        ViewBag.SearchString = searchString; // Pass searchString to ViewBag for persistence
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalCount = await _eventsService.GetTotalCountAsync(searchString); // Assuming you have a method to get total count


        return View(events);
    }

    public IActionResult Details(string id)
    {
        var events = _eventsService.GetAsync(id).Result;
        return View(events);
    }
    public ActionResult Create()
    {
        Console.WriteLine("here2");
        return View();
    }

    public ActionResult Edit(string id)
    {
        var events = _eventsService.GetAsync(id).Result;
        Console.WriteLine(events.Status);
        return View(events);
    }

    public ActionResult Delete(string id)
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
        Console.WriteLine("here1");
        try
        {
            string message_response;
            if (ModelState.IsValid)
            {
				if (tangti.Services.UtilsService.validateErrorTime(events.EventDate, events.EnrollDate) != "")
				{
					ViewBag.Message = tangti.Services.UtilsService.validateErrorTime(events.EventDate, events.EnrollDate);
					return (View());
				}
				else{
                	events.Id = ObjectId.GenerateNewId().ToString();
                	await _eventsService.CreateAsync(events);

                    Enroll newEnroll = new Enroll{
                        EventID = events.Id.ToString(),
                        Id = ObjectId.GenerateNewId().ToString(),
                        Member = 0
                    };

                    await _enrollService.CreateAsync(newEnroll);

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
        if (tangti.Services.UtilsService.ValidateErrorTime(updateEvent.EventDate, updateEvent.EnrollDate) != "")
        {
            ViewBag.Message = tangti.Services.UtilsService.ValidateErrorTime(updateEvent.EventDate, updateEvent.EnrollDate);
            return (View());
        }
        await _eventsService.UpdateAsync(id, updateEvent);
        return RedirectToAction("Index");
    }
    [HttpPost]
    public async Task<IActionResult> DeleteAsync(string id)
    {
        var _event = await _eventsService.GetAsync(id);

        if (_event == null) // Check if event is null
        {
            return NotFound();
        }

        await _eventsService.DeleteAsync(id); // Delete using the provided id

        return RedirectToAction("Index");
    }
}