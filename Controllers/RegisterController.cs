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
            // Perform validation here if needed
            if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            {
                ViewBag.Message = "Please fill in all fields.";
                return View("Index");
            }

            // Perform further validation (e.g., check if email format is valid)

            // If validation passes, you can process the registration (e.g., save to database)
            // For demonstration purposes, we'll just display a success message
            ViewBag.Message = "Registration successful!";
            return View("Index");
        }
    }
}
