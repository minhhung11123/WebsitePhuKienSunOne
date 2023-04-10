using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WebsitePhuKienSunOne.Models;

namespace WebsitePhuKienSunOne.Controllers
{
    public class LocationController : Controller
    {
        private readonly dbSunOneContext _context;
        public LocationController(dbSunOneContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public ActionResult ListDistrict(int id)
        {
            var city = _context.Locations.SingleOrDefault(x=>x.LocationId == id);
            var districts = _context.Locations
                .Where(x => x.ParentCode == city.Code && x.Levels == 1)
                .OrderBy(x => x.Code)
                .ToList();
            return Json(districts);
        }
        public ActionResult ListWard(int id)
        {
			var district = _context.Locations.SingleOrDefault(x => x.LocationId == id);
			var wards = _context.Locations
                .Where(x => x.ParentCode == district.Code && x.Levels == 2)
                .OrderBy(x => x.Code)
                .ToList();
            return Json(wards);
        }
    }
}
