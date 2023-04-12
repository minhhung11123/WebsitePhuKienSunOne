using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebsitePhuKienSunOne.Models;

namespace WebsitePhuKienSunOne.Controllers
{
    public class PageController : Controller
    {
        private readonly dbSunOneContext _context;

        public PageController(dbSunOneContext context)
        {
            _context = context;
        }
        
        [Route("page-{Alias}", Name = "PageDetails")]
        public IActionResult Details(string Alias)
        {
            if (string.IsNullOrEmpty(Alias)) return RedirectToAction("Index", "Home");
            var page = _context.Pages.AsNoTracking().SingleOrDefault(x => x.Alias == Alias);
            if(page == null) return RedirectToAction("Index", "Home");
            return View(page);
        }
    }
}
