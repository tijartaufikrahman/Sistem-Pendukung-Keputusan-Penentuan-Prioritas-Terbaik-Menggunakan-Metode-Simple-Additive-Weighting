using Microsoft.AspNetCore.Mvc;

namespace SistemSPK.Controllers
{
    public class AdminController : Controller
    {
        [HttpGet]
        public IActionResult Dashboard()
        {
            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        public IActionResult Database()
        {
            ViewData["Title"] = "Database";
            return View();
        }
    }
}
