using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using WebsitePhuKienSunOne.Models;

namespace WebsitePhuKienSunOne.Controllers
{
    public class ProductController : Controller
    {
        private readonly dbSunOneContext _context;

        public ProductController(dbSunOneContext context)
        {
            _context = context;
        }

        [Route("/shop.html", Name = "ShopProduct")]
        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 9;
            var lsProduct = _context.Products
                        .AsNoTracking()
                        .Where(x => x.Active == true)
                        .OrderByDescending(x => x.DateCreated);

            PagedList<Product> models = new PagedList<Product>(lsProduct, pageNumber, pageSize);

            var lsCat = _context.Categories.AsNoTracking().Where(x => x.Alias != "khac" && x.Published == true).ToList();

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPage = models.PageCount;
            ViewBag.ListCat = lsCat;
            return View(models);
        }

        [Route("/shop-{Alias}", Name = "ListProduct")]
        public IActionResult List(string Alias, int page = 1)
        {
            try
            {
                var pageSize = 9;
                var cat = _context.Categories.AsNoTracking().SingleOrDefault(x => x.Alias == Alias);
                var lsProduct = _context.Products
                            .AsNoTracking()
                            .Where(x => x.CatId == cat.CatId && x.Active == true)
                            .OrderByDescending(x => x.DateCreated);

                PagedList<Product> models = new PagedList<Product>(lsProduct, page, pageSize);

                var lsCat = _context.Categories.AsNoTracking().Where(x => x.Alias != "khac" && x.Published == true).ToList();

                ViewBag.CurrentPage = page;
                ViewBag.CurrentCat = cat;
                ViewBag.TotalPage = models.PageCount;
                ViewBag.ListCat = lsCat;
                return View(models);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Route("/{Alias}-{id}.html", Name = "ProductDetails")]
        public IActionResult Details(int id)
        {
            try
            {
                var product = _context.Products.Include(x => x.Cat).FirstOrDefault(x => x.ProductId == id);
                if (product == null)
                {
                    return RedirectToAction("Index");
                }
                var lsProduct = _context.Products
                .AsNoTracking()
                .Where(x => x.Active == true && x.ProductId != id && x.CatId == product.CatId)
                .Take(4)
                .OrderByDescending(x => x.DateCreated)
                .ToList();
                ViewBag.ListProduct = lsProduct;
                return View(product);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
