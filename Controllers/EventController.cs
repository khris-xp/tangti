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
			tangti.Models.Event e1 = new tangti.Models.Event()
			{
				Name = "Event 1",
				Status = "Ongoing",
				Limit = 10
			};
			return View(e1);
		}
	}
}