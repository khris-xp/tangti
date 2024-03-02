using tangti.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace tangti.Controllers
{
	public class Event: Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Debug()
		{
			Event e1 = new Event()
			{
				Name = "Event 1",
				Status = "Ongoing",
				Limit = 10
			};
			return View();
		}
	}
}