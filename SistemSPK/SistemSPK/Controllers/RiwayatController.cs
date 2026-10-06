using Microsoft.AspNetCore.Mvc;

namespace SistemSPK.Controllers
{
    public class RiwayatController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Riwayat Penilaian Alternatif";
            return View();
        }

        public IActionResult Detail(string? id)
        {
            ViewData["Title"] = "Detail Riwayat Penilaian";
            ViewData["KeputusanId"] = id ?? "";
            return View();
        }
    }
}
