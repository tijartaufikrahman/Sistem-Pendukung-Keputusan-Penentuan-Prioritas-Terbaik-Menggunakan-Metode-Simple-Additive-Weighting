using Microsoft.AspNetCore.Mvc;

namespace SistemSPK.Controllers
{
    public class KeputusanController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Penentuan Prioritas Alternatif";
            return View();
        }
    }
}
