using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using WebsitePhuKienSunOne.Extension;
using WebsitePhuKienSunOne.Models;
using WebsitePhuKienSunOne.ModelViews;

namespace WebsitePhuKienSunOne.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly dbSunOneContext _context;
        public INotyfService _notifyService { get; }
        public ShoppingCartController(dbSunOneContext context, INotyfService notyf)
        {
            _context = context;
            _notifyService = notyf;
        }
        public List<CartItem> Cart
        {
            get
            {
                var cart = HttpContext.Session.Get<List<CartItem>>("Cart");
                if (cart == default(List<CartItem>))
                {
                    cart = new List<CartItem>();
                }
                return cart;
            }
        }

        [HttpPost]
        [Route("api/cart/add")]
        public IActionResult AddToCart(int productID, int? amount)
        {
            List<CartItem> cart = Cart;
            try
            {
                CartItem item = cart.SingleOrDefault(p => p.product.ProductId == productID);
                if (item != null)
                {
                    if (amount.HasValue)
                    {
                        item.amount += amount.Value;
                    }
                    else
                    {
                        item.amount++;
                    }
                    Product product = _context.Products.SingleOrDefault(p => p.ProductId == productID);
                    if (item.amount > product.UnitslnStock)
                    {
                        return Json(new { success = false });
                    }
                }
                else
                {
                    Product product = _context.Products.SingleOrDefault(p => p.ProductId == productID);
                    item = new CartItem
                    {
                        amount = amount.HasValue ? amount.Value : 1,
                        product = product
                    };
                    if (item.amount > product.UnitslnStock)
                    {
                        return Json(new { success = false });
                    }
                    cart.Add(item);
                }
                HttpContext.Session.Set<List<CartItem>>("Cart", cart);
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        [Route("api/cart/remove")]
        public ActionResult Remove(int productID)
        {
            try
            {
                List<CartItem> cart = Cart;
                CartItem item = cart.SingleOrDefault(p => p.product.ProductId == productID);
                if (item != null)
                {
                    cart.Remove(item);
                }
                HttpContext.Session.Set<List<CartItem>>("Cart", cart);
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        [Route("api/cart/update")]
        public IActionResult UpdateCart(int productID, int? amount)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("Cart");
            try
            {
                if (cart != null)
                {
                    CartItem item = cart.SingleOrDefault(p => p.product.ProductId == productID);
                    if (item != null && amount.HasValue)
                    {
                        item.amount = amount.Value;
                    }
                    Product product = _context.Products.SingleOrDefault(p => p.ProductId == productID);
                    if (item.amount > product.UnitslnStock)
                    {
                        return Json(new { success = false });
                    }
                    HttpContext.Session.Set<List<CartItem>>("Cart", cart);
                }
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        [Route("Cart.html", Name = "Cart")]
        public IActionResult Index()
        {
            return View(Cart);
        }
    }
}
