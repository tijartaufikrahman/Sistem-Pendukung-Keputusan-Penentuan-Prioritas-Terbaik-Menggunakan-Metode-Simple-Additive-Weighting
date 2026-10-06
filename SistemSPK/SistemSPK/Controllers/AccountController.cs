using Microsoft.AspNetCore.Mvc;

namespace SistemSPK.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult VerifyOtp(string? email = null)
        {
            ViewData["Email"] = email ?? "";
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string? token = null, string? email = null, string? otp = null)
        {
            ViewData["Token"] = token ?? "";
            ViewData["Email"] = email ?? "";
            ViewData["Otp"] = otp ?? "";
            return View();
        }
    }
}
