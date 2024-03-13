using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using tangti.Models;
using tangti.Services;

namespace tangti.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
	private readonly EventService _eventsService;
	private readonly EnrollService _enrollService;

    public HomeController(ILogger<HomeController> logger, EventService eventsService, EnrollService enrollService)
    {
        _logger = logger;
		_eventsService = eventsService;
		_enrollService = enrollService;
    }

    public async Task<IActionResult> Index()
    {
		var events = _eventsService.GetAsync().Result;
		foreach (var curr_event in events)
		{
			Console.WriteLine(curr_event.Title);
			if (curr_event.Id != null)
            {
                var enroll = _enrollService.GetEventEnrollAsync(curr_event.Id).Result;
                if (enroll != null)
                {
                    curr_event.members = enroll.Member;
                }
            }
			if (curr_event.Type != "Queue" && curr_event.Status != "CLOSED")
				await _eventsService.changeStatus(curr_event.Id, "CLOSED");
			if (curr_event.Status != "NOT_OPENED" && await _eventsService.isTimeNotOpen(curr_event.Id))
				await _eventsService.changeStatus(curr_event.Id, "NOT_OPENED");
			if (curr_event.Status != "ON_GOING" && await _eventsService.isEnrollTime(curr_event.Id))
				await _eventsService.changeStatus(curr_event.Id, "ON_GOING");
			
		}
        return View(events);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
