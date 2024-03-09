using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using tangti.Models;
using tangti.Services;

namespace tangti.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
	private readonly EventService _eventsService;

    public HomeController(ILogger<HomeController> logger, EventService eventsService)
    {
        _logger = logger;
		_eventsService = eventsService;
    }

    public IActionResult Index()
    {
		var events = _eventsService.GetAsync().Result;
		foreach (var curr_event in events)
		{
			Console.WriteLine(curr_event.Title);
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
