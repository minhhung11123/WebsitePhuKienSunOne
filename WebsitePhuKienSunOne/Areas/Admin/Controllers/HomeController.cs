using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebsitePhuKienSunOne.Models;

namespace WebsitePhuKienSunOne.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("RequireAdminRole")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var adminId = HttpContext.Session.GetString("AdminId");
            if (adminId == null)
            {
                return RedirectToAction("Login", "AdminLogin");
            }
            return View();
        }
    }
}
