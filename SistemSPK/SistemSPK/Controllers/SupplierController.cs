using Microsoft.AspNetCore.Mvc;

namespace SistemSPK.Controllers
{
    public class SupplierController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Data Alternatif";
            return View();
        }
    }
}
