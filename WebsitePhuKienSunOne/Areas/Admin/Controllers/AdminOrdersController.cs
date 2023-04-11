using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebsitePhuKienSunOne.Models;

namespace WebsitePhuKienSunOne.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("RequireAdminRole")]
    public class AdminOrdersController : Controller
    {
        private readonly dbSunOneContext _context;
        private INotyfService _notyf;

        public AdminOrdersController(dbSunOneContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
        }

        // GET: Admin/AdminOrders
        public async Task<IActionResult> Index()
        {
            var dbSunOneContext = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.TransactStatus)
                .AsNoTracking()
                .OrderByDescending(x => x.OrderDate);
            return View(await dbSunOneContext.ToListAsync());
        }

        // GET: Admin/AdminOrders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.CityNavigation)
                .Include(o => o.Customer)
                .Include(o => o.DistrictNavigation)
                .Include(o => o.TransactStatus)
                .Include(o => o.WardNavigation)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            var orderDetail = _context.OrderDetails
                .AsNoTracking()
                .Include(x => x.Product)
                .Where(x => x.OrderId == order.OrderId)
                .OrderBy(x => x.OrderDetailId)
                .ToList();
            ViewBag.OrderDetail = orderDetail;
            return View(order);
        }

        public async Task<IActionResult> ChangeStatus(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.CityNavigation)
                .Include(o => o.Customer)
                .Include(o => o.DistrictNavigation)
                .Include(o => o.TransactStatus)
                .Include(o => o.WardNavigation)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }
            var orderDetail = _context.OrderDetails
               .AsNoTracking()
               .Include(x => x.Product)
               .Where(x => x.OrderId == order.OrderId)
               .OrderBy(x => x.OrderDetailId)
               .ToList();
            ViewBag.OrderDetail = orderDetail;
            ViewData["Status"] = new SelectList(_context.TransactStatuses, "TransactStatusId", "Status", order.TransactStatusId);
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int id, int TransactStatusId, bool paid)
        {
            try
            {
                var order = await _context.Orders.FindAsync(id);
                if (order == null) { return NotFound(); }
                order.TransactStatusId = TransactStatusId;
                if (TransactStatusId == 3)
                {
                    order.ShipDate = DateTime.Now;
                }
                if (!order.Paid)
                {
                    if (paid)
                    {
                        order.Paid = true;
                        order.PaymentDate = DateTime.Now;
                    }
                }
                if(TransactStatusId == 5)
                {
                    order.Deleted = true;
                }
                _context.Update(order);
                await _context.SaveChangesAsync();
                _notyf.Success("Cập nhật trạng thái đơn hàng thành công");
            }
            catch
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Index");
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
