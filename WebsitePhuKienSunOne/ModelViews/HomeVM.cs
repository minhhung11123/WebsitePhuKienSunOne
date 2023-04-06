using System.Collections.Generic;
using WebsitePhuKienSunOne.Models;

namespace WebsitePhuKienSunOne.ModelViews
{
    public class HomeVM
    {
        public List<Product> Products { get; set; }
        public List<Category> Category { get; set; }
    }
}
