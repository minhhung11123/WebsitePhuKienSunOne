using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebsitePhuKienSunOne.Models;

namespace WebsitePhuKienSunOne.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("RequireAdminRole")]
    public class AdminLocationsController : Controller
    {
        private readonly dbSunOneContext _context;
        private INotyfService _notyfService;

        public AdminLocationsController(dbSunOneContext context, INotyfService notyfService)
        {
            _notyfService = notyfService;
            _context = context;
        }

        // GET: Admin/AdminLocations
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "AdminLogin");
            }
            return View(await _context.Locations.ToListAsync());
        }        

        // GET: Admin/AdminLocations/Create
        public IActionResult Create()
        {
            
            return View();
        }

        // POST: Admin/AdminLocations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LocationId,Name,Type,Slug,ParentCode,Levels,Code")] Location location)
        {
            if (ModelState.IsValid)
            {
                _context.Add(location);
                await _context.SaveChangesAsync();
                _notyfService.Success("Thêm mới thành công");
                return RedirectToAction(nameof(Index));
            }
            return View(location);
        }

        // GET: Admin/AdminLocations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _context.Locations.FindAsync(id);
            if (location == null)
            {
                return NotFound();
            }
            ViewBag.Select = selectLocation(location.ParentCode, location.Levels);
            return View(location);
        }

        // POST: Admin/AdminLocations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LocationId,Name,Type,Slug,ParentCode,Levels,Code")] Location location)
        {
            if (id != location.LocationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _notyfService.Success("Cập nhật thành công");
                    _context.Update(location);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LocationExists(location.LocationId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(location);
        }

        private bool LocationExists(int id)
        {
            return _context.Locations.Any(e => e.LocationId == id);
        }

		public ActionResult ListLocation(int levels)
		{
            if (levels == 1)
            {
				var lsLocation = _context.Locations
					.Where(x=>x.Levels == 0)
					.OrderBy(x => x.Code)
					.ToList();
				return Json(lsLocation);
			}
			if (levels == 2)
            {
				var lsLocation = _context.Locations
					.Where(x => x.Levels == 1)
					.OrderBy(x => x.Code)
					.ToList();
				return Json(lsLocation);
			}
            return Json(null);
		}
        
        public Location selectLocation(int? code, int? levels)
        {
            levels--;
            return _context.Locations
                .AsNoTracking()
                .FirstOrDefault(x=>x.Code == code && x.Levels == levels);
        }
	}
}
