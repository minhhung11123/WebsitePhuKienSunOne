﻿using Microsoft.AspNetCore.Mvc;

namespace WebsitePhuKienSunOne.Controllers
{
    public class AjaxContentController : Controller
    {
        public IActionResult HeaderCart()
        {
            return ViewComponent("HeaderCart");
        }
        public IActionResult NumberCart()
        {
            return ViewComponent("NumberCart");
        }
    }
}
