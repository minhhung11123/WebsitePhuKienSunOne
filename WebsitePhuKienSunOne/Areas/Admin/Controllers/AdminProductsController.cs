using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebsitePhuKienSunOne.Helpper;
using WebsitePhuKienSunOne.Models;

namespace WebsitePhuKienSunOne.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("RequireAdminRole")]
    public class AdminProductsController : Controller
    {
        private readonly dbSunOneContext _context;
        public INotyfService _notifyService { get; }


        public AdminProductsController(dbSunOneContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

        // GET: Admin/AdminProducts
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("AdminId") == null)
            {
                return RedirectToAction("Login", "AdminLogin");
            }
            var dbSunOneContext = _context.Products.Include(p => p.Cat);
            return View(await dbSunOneContext.ToListAsync());
        }

        // GET: Admin/AdminProducts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Cat)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // GET: Admin/AdminProducts/Create
        public IActionResult Create()
        {
            ViewData["CatName"] = new SelectList(_context.Categories, "CatId", "CatName");
            return View();
        }

        // POST: Admin/AdminProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,ShortDesc,Description,CatId,Price,Discount,Thumb,Video,DateCreated,DateModified,BestSellers,HomeFlag,Active,Tags,Title,Alias,MetaDesc,MetaKey,UnitslnStock")] Product product, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (ModelState.IsValid)
            {
                product.ShortDesc = Utilities.ToTitleCase(product.ShortDesc);
                if(fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    string image = Utilities.SEOUrl(product.ShortDesc) + extension;
                    product.Thumb = await Utilities.UploadFile(fThumb, @"products", image.ToLower());
                }
                if (string.IsNullOrEmpty(product.Thumb)) product.Thumb = "default.jpg";
                product.Alias = Utilities.SEOUrl(product.ShortDesc);
                product.DateCreated = DateTime.Now;
                product.DateModified = DateTime.Now;
                _context.Add(product);
                await _context.SaveChangesAsync();
                _notifyService.Success("Thêm sản phẩm thành công");
                return RedirectToAction(nameof(Index));
            }
            ViewData["CatId"] = new SelectList(_context.Categories, "CatId", "CatId", product.CatId);
            return View(product);
        }

        // GET: Admin/AdminProducts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CatName"] = new SelectList(_context.Categories, "CatId", "CatName", product.CatId);
            return View(product);
        }

        // POST: Admin/AdminProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,ShortDesc,Description,CatId,Price,Discount,Thumb,Video,DateCreated,DateModified,BestSellers,HomeFlag,Active,Tags,Title,Alias,MetaDesc,MetaKey,UnitslnStock")] Product product, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    product.ShortDesc = Utilities.ToTitleCase(product.ShortDesc);
                    if (fThumb != null)
                    {
                        string extension = Path.GetExtension(fThumb.FileName);
                        string image = Utilities.SEOUrl(product.ShortDesc) + extension;
                        product.Thumb = await Utilities.UploadFile(fThumb, @"products", image.ToLower());
                    }
                    if (string.IsNullOrEmpty(product.Thumb)) product.Thumb = "default.jpg";
                    product.Alias = Utilities.SEOUrl(product.ShortDesc);
                    product.DateModified = DateTime.Now;
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    _notifyService.Success("Cập nhật sản phẩm thành công");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            ViewData["CatId"] = new SelectList(_context.Categories, "CatId", "CatId", product.CatId);
            return View(product);
        }

        // GET: Admin/AdminProducts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Cat)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/AdminProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            _notifyService.Success("Xóa sản phẩm thành công");
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
