using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using WebsitePhuKienSunOne.Extension;
using WebsitePhuKienSunOne.ModelViews;

namespace WebsitePhuKienSunOne.Controllers.Components
{
    public class HeaderCartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("Cart");
            return View(cart);
        }
    }
}
