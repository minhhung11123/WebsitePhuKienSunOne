using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Notyf;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebsitePhuKienSunOne.Helpper;
using WebsitePhuKienSunOne.Models;
using WebsitePhuKienSunOne.ModelViews;

namespace WebsitePhuKienSunOne.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly dbSunOneContext _context;
        public INotyfService _notifyService { get; }


        public HomeController(ILogger<HomeController> logger, dbSunOneContext context, INotyfService notifyService)
        {
            _logger = logger;
            _context = context;
            _notifyService = notifyService;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("CustomerId") == null)
            {
                HttpContext.SignOutAsync();
            };

            HomeVM model = new HomeVM();

            var lsProducts = _context.Products
                .AsNoTracking()
                .Where(x => x.Active == true && x.HomeFlag == true)
                .OrderByDescending(x => x.DateCreated)
                .ToList();

            var lsCat = _context.Categories
                .AsNoTracking()
                .Where(x => x.Published == true)
                .OrderBy(x => x.Ordering)
                .Take(3)
                .ToList();

            model.Products = lsProducts;
            model.Category = lsCat;
            ViewBag.model = model;
            return View();
        }

        [Route("contact.html", Name = "Contact")]
        public IActionResult Contact()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
