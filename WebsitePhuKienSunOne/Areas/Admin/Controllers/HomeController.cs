using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Utilities;
using System;
using System.Linq;
using WebsitePhuKienSunOne.Models;

namespace WebsitePhuKienSunOne.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("RequireAdminRole")]
    public class HomeController : Controller
    {
        private readonly dbSunOneContext _context;
        private INotyfService _notyf;
        public HomeController(dbSunOneContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "AdminLogin");
            }

            ViewBag.TotalCustomer = _context.Customers.Count();
            ViewBag.TotalProduct = _context.Products.Count();
            ViewBag.TotalSold = _context.OrderDetails.Sum(x => x.Quantity);
            ViewBag.TotalMoney = _context.Orders.Sum(x => x.TotalMoney);
            int[,] value = new int[15, 2];
            var dailyTotals = _context.OrderDetails
                        .Where(x => x.CreateDate.Date > DateTime.Now.Date.AddDays(-15).Date)
                        .GroupBy(x => x.CreateDate.Date)
                        .Select(g => new
                        {
                            Date = g.Key,
                            TotalQuantity = g.Sum(x => x.Quantity)
                        })
                        .ToList();
            for (int i = 15; i >= 1; i--)
            {
                value[i - 1, 0] = i;
                value[i - 1, 1] = 0;
            }

            foreach (var item in dailyTotals)
            {
                var a = (DateTime.Now.Date - item.Date.Date).Days + 1;
                value[a, 1] = item.TotalQuantity.Value;
            }

            string str = "[";

            for (int i = 0; i < value.GetLength(0); i++)
            {
                str += "[" + value[i, 0] + "," + value[i, 1] + "]";
                if (i < value.GetLength(0) - 1)
                {
                    str += ",";
                }
            }

            str += "]";
            ViewBag.List = str;
            return View();
        }
    }
}
