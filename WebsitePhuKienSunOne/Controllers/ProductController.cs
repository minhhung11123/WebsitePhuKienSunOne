using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            var product = _context.Products.Include(x=>x.Cat).FirstOrDefault(x=>x.ProductId==id);
            if(product == null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
