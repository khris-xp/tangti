using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using tangti.Models;

namespace tangti.Controllers
{
    public class RegisterController : Controller
    {
        // GET: Register
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            {
                ViewBag.Message = "Please fill in all fields.";
                return View("Index");
            }
            ViewBag.Message = "Registration successful!";
            return View("Index");
        }
    }
}
