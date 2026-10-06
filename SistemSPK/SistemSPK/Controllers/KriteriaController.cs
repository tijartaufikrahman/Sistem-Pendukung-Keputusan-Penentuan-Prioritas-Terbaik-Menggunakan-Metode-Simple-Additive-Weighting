using System;
using Microsoft.AspNetCore.Mvc;

namespace SistemSPK.Controllers
{
    public class KriteriaController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Aspek Penilaian Evaluasi";
            return View();
        }

        public IActionResult Parameter(Guid? aspekId)
        {
            ViewData["Title"] = "Kriteria Penilaian";
            ViewData["AspekId"] = aspekId?.ToString() ?? string.Empty;
            return View();
        }
    }
}
