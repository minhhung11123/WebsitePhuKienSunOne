﻿using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
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

        [Route("Profile.html", Name = "Profile")]
        public IActionResult Profile()
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

        [Route("Dashboard.html", Name = "Dashboard")]
        public IActionResult Dashboard()
        {
            var accountId = HttpContext.Session.GetString("CustomerId");
            if (accountId != null)
            {
                var cs = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(accountId));
                if (cs != null)
                {
                    var lsOrder = _context.Orders
                        .AsNoTracking()
                        .Include(x => x.TransactStatus)
                        .Where(x => x.CustomerId == Convert.ToInt32(accountId))
                        .OrderByDescending(x => x.OrderDate)
                        .ToList();
                    return View(lsOrder);
                }
            }
            return RedirectToAction("Login");
        }

        [Route("ViewDetail-{id}", Name = "ViewDetail")]
        public IActionResult ViewDetail(int id)
        {
            var csID = HttpContext.Session.GetString("CustomerId");
            var order = _context.Orders
                .AsNoTracking()
                .Include(x => x.Customer)
                .Include(x => x.TransactStatus)
                .Include(x => x.WardNavigation)
                .Include(x => x.CityNavigation)
                .Include(x => x.DistrictNavigation)
                .FirstOrDefault(x => x.OrderId == id && x.CustomerId == Int32.Parse(csID));
            if (order == null)
            {
                return NotFound();
            }
            var orderDetails = _context.OrderDetails
                .AsNoTracking()
                .Include(x => x.Product)
                .Where(x => x.OrderId == id)
                .ToList();
            ViewBag.OrderDetails = orderDetails;
            return View(order);
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
                    var ac = _context.Customers.FirstOrDefault(c => c.Email == customer.Email.Trim().ToLower());
                    if (ac == null)
                    {
                        string salt = Utilities.GetRandomCode();
                        Customer cs = new Customer
                        {
                            FullName = customer.FullName,
                            Phone = customer.Phone.Trim().ToLower(),
                            Email = customer.Email.Trim().ToLower(),
                            Password = (customer.Password + salt.Trim()).ToMD5(),
                            Avatar = "default.jpg",
                            Birthday = customer.BirthDay,
                            Active = true,
                            Salt = salt,
                            CrateDate = DateTime.Now
                        };
                        try
                        {
                            _context.Add(cs);
                            await _context.SaveChangesAsync();
                            _notifyService.Success("Đăng ký thành công");
                            return RedirectToAction("Login", "Accounts");
                        }
                        catch
                        {
                            return RedirectToAction("Register", "Accounts");
                        }
                    }
                    else
                    {
                        _notifyService.Error("Email đã được đăng ký");
                        return View(customer);
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
                return RedirectToAction("Profile", "Accounts");
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
                    if (cs == null)
                    {
                        _notifyService.Warning("Sai thông tin đăng nhập");
                        return View(customer);
                    }
                    string pass = (customer.Password + cs.Salt.Trim()).ToMD5();
                    if (cs.Password != pass)
                    {
                        _notifyService.Warning("Sai thông tin đăng nhập");
                        return View(customer);
                    }
                    if (!cs.Active)
                    {
                        _notifyService.Warning("Tài khoản của bạn đã bị khóa");
                        return View(customer);
                    }
                    cs.LastLogin = DateTime.Now;
                    _context.Update(cs);
                    _context.SaveChanges();
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
        [Route("Logout.html", Name = "Logout")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            HttpContext.Session.Remove("CustomerId");
            HttpContext.Session.Remove("CustomerName");
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAvatar(IFormFile avatar)
        {
            var customerId = HttpContext.Session.GetString("CustomerId");
            if (customerId != null)
            {
                var customer = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(customerId));
                if (customer == null)
                {
                    RedirectToAction("Login");
                }
                if (avatar != null)
                {
                    string extension = Path.GetExtension(avatar.FileName);
                    string image = Utilities.SEOUrl(customer.FullName) + extension;
                    customer.Avatar = await Utilities.UploadFile(avatar, @"customers", image.ToLower());
                }
                if (string.IsNullOrEmpty(customer.Avatar)) customer.Avatar = "default.jpg";
                _context.Update(customer);
                await _context.SaveChangesAsync();
                _notifyService.Success("Cập nhật ảnh đại diện thành công");
                return RedirectToAction("Profile");
            }
            return RedirectToAction("Login");
        }

        [HttpGet]
        [Route("ChangePassword.html", Name = "ChangePassword")]
        public IActionResult ChangePassword()
        {
            var accountId = HttpContext.Session.GetString("CustomerId");
            if (accountId != null)
            {
                var cs = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(accountId));
                if (cs != null)
                {
                    return View();
                }
            }
            return RedirectToAction("Login");
        }

        [HttpPost]
        [Route("ChangePassword.html", Name = "ChangePassword")]
        public IActionResult ChangePassword(ChangePasswordVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var customerId = HttpContext.Session.GetString("CustomerId");
                    if (customerId != null)
                    {
                        var customer = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(customerId));
                        if (customer == null)
                        {
                            RedirectToAction("Login");
                        }
                        var pass = (model.OldPassword.Trim() + customer.Salt.Trim()).ToMD5();
                        if (pass == customer.Password)
                        {
                            string newpass = (model.NewPassword.Trim() + customer.Salt.Trim()).ToMD5();
                            customer.Password = newpass;
                            _context.Update(customer);
                            _context.SaveChanges();
                            _notifyService.Success("Đổi mật khẩu thành công");
                            return RedirectToAction("Profile");
                        }
                        else
                        {
                            _notifyService.Error("Mật khẩu cũ không chính xác");
                            return RedirectToAction("ChangePassword");
                        }
                    }
                    return RedirectToAction("Login");
                }
                else
                {
                    return View();
                }
            }
            catch
            {
                _notifyService.Error("Đổi mật khẩu không thành công");
                return RedirectToAction("Profile");
            }
        }

        [HttpGet]
        [Route("ForgetPassword.html", Name = "ForgetPassword")]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [Route("ForgetPassword.html", Name = "ForgetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Customers.AsNoTracking().FirstOrDefault(c => c.Email == model.Email);

                if (user != null)
                {
                    // Generate password reset token
                    var token = Guid.NewGuid().ToString();
                    user.Token = token;
                    user.TokenTimeOut = DateTime.Now.AddMinutes(10);
                    _context.Update(user);
                    _context.SaveChanges();

                    // Send password reset email
                    var callbackUrl = Url.Action("ResetPassword", "Accounts", new { userId = user.CustomerId, token = token }, protocol: HttpContext.Request.Scheme);

                    var smtpClient = new SmtpClient("smtp.gmail.com", 587)
                    {
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential("minhhung111234@gmail.com", "hdzaihissvlzthcj"),
                        EnableSsl = true,
                    };

                    var message = new MailMessage("minhhung111234@gmail.com", user.Email, "Lấy lại mật khẩu", $"Để lấy lại mật khẩu vui lòng nhấn vào đây: {callbackUrl}");
                    await smtpClient.SendMailAsync(message);

                    return RedirectToAction("ForgotPasswordConfirmation", "Accounts");
                }

                // To indicate email not found, always show success page.
                return RedirectToAction("ForgotPasswordConfirmation", "Accounts");
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("ForgotPasswordConfirmation.html", Name = "ForgotPasswordConfirmation")]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("ResetPassword", Name = "ResetPassword")]
        public IActionResult ResetPassword(string token, string userId)
        {
            ResetPasswordVM model = new ResetPasswordVM { Token = token, CustomerID = Int32.Parse(userId) };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("ResetPassword", Name = "ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Customers.AsNoTracking().FirstOrDefault(c => c.CustomerId == model.CustomerID);

                if (user != null)
                {
                    if (user.Token == model.Token || user.TokenTimeOut > DateTime.Now)
                    {
                        user.Password = (model.Password.Trim() + user.Salt.Trim()).ToMD5();
                        _context.Update(user);
                        await _context.SaveChangesAsync();
                        _notifyService.Success("Mật khẩu đã được thay đổi");
                        return RedirectToAction("Login", "Accounts");
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            return View(model);
        }
    }
}
