using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using WebsitePhuKienSunOne.ModelViews;
using AspNetCoreHero.ToastNotification.Abstractions;
using WebsitePhuKienSunOne.Models;
using System.Linq;
using WebsitePhuKienSunOne.Extension;

namespace WebsitePhuKienSunOne.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("RequireAdminRole")]
    public class AdminLoginController : Controller
	{
		private readonly dbSunOneContext _context;
		private INotyfService _notifyService;

		public AdminLoginController(dbSunOneContext context, INotyfService notyfService)
		{
			_context = context;
			_notifyService = notyfService;
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult Login(string returnUrl = null)
		{
            var accountId = HttpContext.Session.GetString("AdminId");
			if (accountId != null)
			{
				return RedirectToAction("Index", "Home");
			}
			return View();
		}
		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> Login(LoginVM account, string returnUrl = null)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var ac = _context.Accounts
						.AsNoTracking()
						.SingleOrDefault(x => x.Email.Trim() == account.Email);
					if (ac == null)
					{
						_notifyService.Warning("Sai thông tin đăng nhập");
						return View(account);
					}
					string pass = (account.Password + ac.Salt.Trim()).ToMD5();
					if (ac.Password != pass)
					{
						_notifyService.Warning("Sai thông tin đăng nhập");
						return View(account);
					}
					if (!ac.Active)
					{
						_notifyService.Warning("Tài khoản của bạn đã bị khóa");
						return View(account);
					}
					ac.LastLogin = DateTime.Now;
					_context.Update(ac);
					_context.SaveChanges();
					HttpContext.Session.SetString("AdminId", ac.AccountId.ToString());
					HttpContext.Session.SetString("AdminName", ac.FullName.ToString());
					var customerId = HttpContext.Session.GetString("AdminId");
					var claims = new List<Claim>
					{
						new Claim(ClaimTypes.Role, "admin"),
						new Claim(ClaimTypes.Name, ac.FullName),
						new Claim("AdminId", customerId)
					};
					ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "AdminLogin");
					ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
					await HttpContext.SignInAsync(claimsPrincipal);
					_notifyService.Success("Đăng nhập thành công");
					return RedirectToAction("Index", "Home");
				}
			}
			catch
			{
				return RedirectToAction("Login", "AdminAccounts");
			}
			return View(account);
		}

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            HttpContext.Session.Remove("AdminId");
            HttpContext.Session.Remove("AdminName");
            return RedirectToAction("Login");
        }
    }
}
