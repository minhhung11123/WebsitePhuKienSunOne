using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using WebsitePhuKienSunOne.Extension;
using WebsitePhuKienSunOne.Models;
using WebsitePhuKienSunOne.ModelViews;

namespace WebsitePhuKienSunOne.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly dbSunOneContext _context;
        public INotyfService _notifyService { get; }
        public CheckoutController(dbSunOneContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }
        public List<CartItem> Cart
        {
            get
            {
                var cart = HttpContext.Session.Get<List<CartItem>>("Cart");
                if (cart == default(List<CartItem>))
                {
                    cart = new List<CartItem>();
                }
                return cart;
            }
        }

        [Route("checkout.html", Name = "Checkout")]
        public IActionResult Index(string returnUrl = null)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("Cart");
            var customerID = HttpContext.Session.GetString("CustomerId");
            CheckoutVM model = new CheckoutVM();
            if (customerID != null)
            {
                var cs = _context.Customers
                    .AsNoTracking()
                    .SingleOrDefault(x => x.CustomerId == Convert.ToInt32(customerID));
                model.CustomerId = cs.CustomerId;
                model.FullName = cs.FullName;
                model.Email = cs.Email;
                model.Phone = cs.Phone;
                model.Address = cs.Address;
            }
            ViewData["lsCity"] = new SelectList(_context.Locations.Where(x => x.Levels == 1).OrderBy(x => x.Type).ToList(), "Tỉnh/Thành");
            ViewBag.Cart = cart;
            return View(model);
        }

        [HttpPost]
        [Route("checkout.html", Name = "checkout")]
        public IActionResult Index(CheckoutVM checkout)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("Cart");
            var customerID = HttpContext.Session.GetString("CustomerId");
            CheckoutVM model = new CheckoutVM();
            if (customerID != null)
            {
                var cs = _context.Customers
                    .AsNoTracking()
                    .SingleOrDefault(x => x.CustomerId == Convert.ToInt32(customerID));
                model.CustomerId = cs.CustomerId;
                model.FullName = cs.FullName;
                model.Email = cs.Email;
                model.Phone = cs.Phone;
                model.Address = cs.Address;
            }
            return View();
        }
    }
}
