using Microsoft.AspNetCore.Mvc;
using CMCS.Models;

namespace CMCS.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Simulate user role for demonstration
            ViewBag.UserRole = "Lecturer"; // Change to "Coordinator" or "Manager" to see different views
            return View();
        }

        public IActionResult Dashboard()
        {
            // Simulate user authentication
            ViewBag.UserName = "Dr. Jan Van Reibeck";
            ViewBag.UserRole = "Lecturer";
            return View();
        }
    }
}