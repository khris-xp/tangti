using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using tangti.Models;

namespace tangti.Controllers
{
	public class Event: Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}