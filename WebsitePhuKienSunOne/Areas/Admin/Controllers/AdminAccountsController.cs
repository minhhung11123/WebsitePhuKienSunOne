﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebsitePhuKienSunOne.Extension;
using WebsitePhuKienSunOne.Helpper;
using WebsitePhuKienSunOne.Models;
using WebsitePhuKienSunOne.ModelViews;

namespace WebsitePhuKienSunOne.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize("RequireAdminRole")]
    public class AdminAccountsController : Controller
	{
		private readonly dbSunOneContext _context;
		private INotyfService _notifyService;


		public AdminAccountsController(dbSunOneContext context, INotyfService notyfService)
		{
			_context = context;
			_notifyService = notyfService;
		}		

		// GET: Admin/AdminAccounts
		public async Task<IActionResult> Index()
		{
			var id = HttpContext.Session.GetString("AdminId");
			var acc = _context.Accounts.AsNoTracking().FirstOrDefault(x => x.AccountId == Int32.Parse(id));
			if (acc.RoleId != 1)
			{
				return RedirectToAction("Index", "Home");
			}
			var dbSunOneContext = _context.Accounts.Include(a => a.Role);
			return View(await dbSunOneContext.ToListAsync());
		}

		// GET: Admin/AdminAccounts/Create
		public IActionResult Create()
		{
			ViewData["Role"] = new SelectList(_context.Roles, "RoleId", "Description");
			return View();
		}

		// POST: Admin/AdminAccounts/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("AccountId,Phone,Email,Password,Salt,Active,FullName,RoleId,LastLogin,CreateDate")] Account account)
		{
			ViewData["Role"] = new SelectList(_context.Roles, "RoleId", "Description", account.RoleId);
			try
			{
				if (ModelState.IsValid)
				{
					string salt = Utilities.GetRandomCode();
					Account ac = new Account
					{
						FullName = account.FullName,
						Phone = account.Phone.Trim().ToLower(),
						Email = account.Email.Trim().ToLower(),
						Password = (account.Password + salt.Trim()).ToMD5(),
						Salt = salt,
						RoleId = account.RoleId,
						CreateDate = DateTime.Now,
						Active = true
					};
					try
					{
						_context.Add(ac);
						await _context.SaveChangesAsync();
						_notifyService.Success("Tạo tài khoản thành công");
						return RedirectToAction("Index");
					}
					catch
					{
						_notifyService.Warning("Tạo tài khoản không thành công");
						return RedirectToAction("Index");
					}
				}
				else
				{
					return RedirectToAction("Index");
				}
			}
			catch
			{
				_notifyService.Warning("Tạo tài khoản không thành công");
				return RedirectToAction("Index");
			}
		}

		// GET: Admin/AdminAccounts/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var account = await _context.Accounts.FindAsync(id);
			if (account == null)
			{
				return NotFound();
			}
			ViewData["Role"] = new SelectList(_context.Roles, "RoleId", "Description", account.RoleId);
			return View(account);
		}

		// POST: Admin/AdminAccounts/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("AccountId,Phone,Email,Password,Salt,Active,FullName,RoleId,LastLogin,CreateDate")] Account account)
		{
			if (id != account.AccountId)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(account);
					await _context.SaveChangesAsync();
					_notifyService.Success("Cập nhật tài khoản thành công");
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!AccountExists(account.AccountId))
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
			ViewData["Role"] = new SelectList(_context.Roles, "RoleId", "Description", account.RoleId);
			return View(account);
		}

		private bool AccountExists(int id)
		{
			return _context.Accounts.Any(e => e.AccountId == id);
		}
	}
}
