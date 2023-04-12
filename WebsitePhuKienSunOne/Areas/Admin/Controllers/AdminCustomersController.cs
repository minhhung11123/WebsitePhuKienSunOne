using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebsitePhuKienSunOne.Extension;
using WebsitePhuKienSunOne.Models;

namespace WebsitePhuKienSunOne.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("RequireAdminRole")]
    public class AdminCustomersController : Controller
    {
        private readonly dbSunOneContext _context;

        public AdminCustomersController(dbSunOneContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminCustomers
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "AdminLogin");
            }
            return View(await _context.Customers.Include(x=>x.Location).ToListAsync());
        }

        // GET: Admin/AdminCustomers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.Include(x=>x.WardNavigation).Include(x=>x.DistrictNavigation).Include(x=>x.Location)
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Admin/AdminCustomers/LockCustomer/5
        public IActionResult LockCustomer(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = _context.Customers
                .AsNoTracking()
                .Include(x => x.WardNavigation)
                .Include(x => x.DistrictNavigation)
                .Include(x => x.Location)
                .FirstOrDefault(x=>x.CustomerId==id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Admin/AdminCustomers/LockCustomer/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LockCustomer(int id, bool Active)
        {
            if (!CustomerExists(id))
            {
                return NotFound();
            }
            var customer = _context.Customers
                        .AsNoTracking()
                        .FirstOrDefault(x => x.CustomerId == id);
            if (ModelState.IsValid)
            {
                try
                {                    
                    customer.Active = Active;
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustomerId))
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
            return View(customer);
        }

        // GET: Admin/AdminCustomers/NewPassword/5
        public IActionResult NewPassword(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = _context.Customers
                .AsNoTracking()
                .Include(x => x.WardNavigation)
                .Include(x => x.DistrictNavigation)
                .Include(x => x.Location)
                .FirstOrDefault(x => x.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Admin/AdminCustomers/NewPassword/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewPassword(int id, string password)
        {
            if (!CustomerExists(id))
            {
                return NotFound();
            }
            var customer = _context.Customers
                        .AsNoTracking()
                        .FirstOrDefault(x => x.CustomerId == id);
            if (ModelState.IsValid)
            {
                try
                {
                    customer.Password = (password.Trim() + customer.Salt.Trim()).ToMD5();
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustomerId))
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
            return View(customer);
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}
