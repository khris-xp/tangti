using tangti.Models;
using tangti.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace tangti.Controllers;

public class EventController : Controller
{
    private readonly EventService _eventsService;

    public EventController(EventService eventsService) =>
        _eventsService = eventsService;

    public IActionResult Index()
    {
        var events = _eventsService.GetAsync().Result;
        return View(events);
    }

    public IActionResult Details(string id)
    {
        var events = _eventsService.GetAsync(id).Result;
        return View(events);
    }
    public ActionResult Create()
    {
        return View();
    }

    public ActionResult Edit(string id)
    {
        var events = _eventsService.GetAsync(id).Result;
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
        try
        {
            string message_response;
            if (ModelState.IsValid)
            {
                events.Id = ObjectId.GenerateNewId().ToString();
                await _eventsService.CreateAsync(events);
                message_response = "Event created successfully";
                ViewBag.Message = message_response;
                return RedirectToAction("Index");
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
    public async Task<IActionResult> Edit(string id, Event events)
    {
        var _event = await _eventsService.GetAsync(id);

        if (_event is null)
        {
            return NotFound();
        }

        await _eventsService.UpdateAsync(id, events);

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