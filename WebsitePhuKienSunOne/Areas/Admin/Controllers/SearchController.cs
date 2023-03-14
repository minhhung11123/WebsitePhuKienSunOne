using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using WebsitePhuKienSunOne.Models;

namespace WebsitePhuKienSunOne.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SearchController : Controller
    {

        private readonly dbSunOneContext _context;
        
        public SearchController(dbSunOneContext context)
        {
            _context = context;
        }

        //GET: Search/FindProduct
        [HttpPost]
        public IActionResult FindProduct(string keyword)
        {
            List<Product> ls = new List<Product>();
            if(string.IsNullOrEmpty(keyword) || keyword.Length < 1)
            {
                return PartialView("ListProductsSearhPartial", null);
            }
            ls = _context.Products
                .AsNoTracking()
                .Include(a => a.Cat)
                .Where(x => x.ProductName.Contains(keyword) || x.ShortDesc.Contains(keyword))
                .OrderByDescending(x => x.ProductName).ThenBy(x => x.ShortDesc)
                .Take(10)
                .ToList();
            if (ls == null)
            {
                return PartialView("ListProductsSearhPartial", null);
            }
            else
            {
                return PartialView("ListProductsSearhPartial", ls);
            }
        }
    }
}
