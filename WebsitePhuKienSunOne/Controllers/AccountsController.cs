using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Threading.Tasks;
using WebsitePhuKienSunOne.Extension;
using WebsitePhuKienSunOne.Helpper;
using WebsitePhuKienSunOne.Models;
using WebsitePhuKienSunOne.ModelViews;

namespace WebsitePhuKienSunOne.Controllers
{
    [Authorize]
    public class AccountsController : Controller
    {
        private readonly dbSunOneContext _context;
        public INotyfService _notifyService { get; }
        public AccountsController(dbSunOneContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

        [Route("dashboard.html", Name = "Dashboard")]
        public IActionResult Dashboard()
        {
            var accountId = HttpContext.Session.GetString("CustomerId");
            if (accountId != null)
            {
                var cs = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(accountId));
                if (cs != null)
                {
                    return View(cs);
                }
            }
            return RedirectToAction("Login");
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("register.html", Name = "Register")]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("register.html", Name = "Register")]
        public async Task<IActionResult> Register(RegisterVM customer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string salt = Utilities.GetRandomCode();
                    Customer cs = new Customer
                    {
                        FullName = customer.FullName,
                        Phone = customer.Phone.Trim().ToLower(),
                        Email = customer.Email.Trim().ToLower(),
                        Password = (customer.Password + salt.Trim()).ToMD5(),
                        Active = true,
                        Salt = salt,
                        CrateDate = DateTime.Now
                    };
                    try
                    {
                        _context.Add(cs);
                        await _context.SaveChangesAsync();
                        HttpContext.Session.SetString("CustomerId", customer.CustomerId.ToString());
                        HttpContext.Session.SetString("CustomerName", customer.FullName.ToString());
                        var customerId = HttpContext.Session.GetString("CustomerId");
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, customer.FullName),
                            new Claim("CustomerId", customerId)
                        };
                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                        await HttpContext.SignInAsync(claimsPrincipal);
                        return RedirectToAction("Dashboard", "Accounts");
                    }
                    catch
                    {
                        return RedirectToAction("Register", "Accounts");
                    }
                }
                else
                {
                    return View(customer);
                }
            }
            catch
            {
                return View(customer);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("login.html", Name = "Login")]
        public IActionResult Login(string returnUrl = null)
        {
            var accountId = HttpContext.Session.GetString("CustomerId");
                if (accountId != null)
            {
                return RedirectToAction("Dashboard", "Accounts");
            }
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("login.html", Name = "Login")]
        public async Task<IActionResult> Login(LoginVM customer, string returnUrl = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var cs = _context.Customers
                        .AsNoTracking()
                        .SingleOrDefault(x => x.Email.Trim() == customer.Email);
                    if (cs == null) return RedirectToAction("Register", "Accounts");
                    string pass = (customer.Password + cs.Salt.Trim()).ToMD5();
                    if (cs.Password != pass)
                    {
                        _notifyService.Warning("Sai thông tin đăng nhập");
                        return View(customer);
                    }
                    if (!cs.Active)
                    {
                        return RedirectToAction("Notify", "Accounts");
                    }
                    HttpContext.Session.SetString("CustomerId", cs.CustomerId.ToString());
                    HttpContext.Session.SetString("CustomerName", cs.FullName.ToString());
                    var customerId = HttpContext.Session.GetString("CustomerId");
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, cs.FullName),
                        new Claim("CustomerId", customerId)
                    };
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);
                    _notifyService.Success("Đăng nhập thành công");
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                return RedirectToAction("Register", "Accounts");
            }
            return View(customer);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidatePhone(string Phone)
        {
            try
            {
                var customer = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Phone.ToLower() == Phone.ToLower());
                if (customer != null)
                {
                    return Json(data: "Số điện thoại : " + Phone + " đã được sử dụng ");
                }
                return Json(data: true);
            }
            catch
            {
                return Json(data: true);
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidateEmail(string Email)
        {
            try
            {
                var customer = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Email.ToLower() == Email.ToLower());
                if (customer != null)
                {
                    return Json(data: "Số điện thoại : " + Email + " đã được sử dụng ");
                }
                return Json(data: true);
            }
            catch
            {
                return Json(data: true);
            }
        }

        [HttpGet]
        [Route("Logout.html", Name ="Logout")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            HttpContext.Session.Remove("CustomerId");
            return RedirectToAction("Index", "Home");
        }
    }
}
