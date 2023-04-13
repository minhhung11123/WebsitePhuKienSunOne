using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection.Metadata.Ecma335;
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
                model.Phone = cs.Phone;
                model.Address = cs.Address;
            }
            ViewBag.lsCity = new SelectList(_context.Locations.Where(x => x.Levels == 0).OrderBy(x => x.Code).ToList(), "LocationId", "Name");
            ViewBag.Cart = cart;
            return View(model);
        }

        [HttpPost]
        [Route("checkout.html", Name = "checkout")]
        public IActionResult Index(CheckoutVM checkout)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("Cart");
            var customerID = HttpContext.Session.GetString("CustomerId");
            if (ModelState.IsValid)
            {
                if (customerID != null)
                {
                    var cs = _context.Customers
                        .AsNoTracking()
                        .SingleOrDefault(x => x.CustomerId == Convert.ToInt32(customerID));
                    cs.FullName = checkout.FullName;
                    cs.Phone = checkout.Phone;
                    cs.Address = checkout.Address;

                    cs.LocationId = checkout.City;
                    cs.District = checkout.District;
                    cs.Ward = checkout.Ward;
                    cs.Address = checkout.Address;

                    _context.Update(cs);
                    _context.SaveChanges();
                }
                try
                {

                    //Create new order
                    Order order = new Order();
                    order.CustomerId = Convert.ToInt32(customerID);
                    order.City = checkout.City;
                    order.District = checkout.District;
                    order.Ward = checkout.Ward;
                    order.OrderDate = DateTime.Now;
                    order.TransactStatusId = 1;
                    order.Deleted = false;
                    order.Paid = false;
                    order.Note = checkout.Note;
                    order.TotalMoney = cart.Sum(x => x.totalMoney);
                    _context.Add(order);
                    _context.SaveChanges();

                    //Create list order detail
                    foreach (var item in cart)
                    {
                        OrderDetail od = new OrderDetail();
                        od.OrderId = order.OrderId;
                        od.ProductId = item.product.ProductId;
                        od.Quantity = item.amount;
                        od.Total = item.totalMoney;
                        od.Price = item.product.Price;
                        od.Discount = item.product.Discount > 0 ? item.product.Discount : null;
                        od.CreateDate = DateTime.Now;
                        _context.Add(od);
                    }
                    _context.SaveChanges();
                    HttpContext.Session.Remove("Cart");
                    return RedirectToAction("Confirmation");
                }
                catch
                {
                    ViewBag.lsCity = new SelectList(_context.Locations.Where(x => x.Levels == 0).OrderBy(x => x.Code).ToList(), "LocationId", "Name");
                    ViewBag.Cart = cart;
                    return View(checkout);
                }
            }
            ViewBag.lsCity = new SelectList(_context.Locations.Where(x => x.Levels == 0).OrderBy(x => x.Code).ToList(), "LocationId", "Name");
            ViewBag.Cart = cart;
            return View(checkout);
        }

        [Route("confirmation.html", Name = "Confirmation")]
        public IActionResult Confirmation()
        {
            return View();
        }
    }
}
