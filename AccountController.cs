using Microsoft.AspNetCore.Mvc;

namespace CMCS.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Logout()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}