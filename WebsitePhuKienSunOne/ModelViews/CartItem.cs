using WebsitePhuKienSunOne.Models;

namespace WebsitePhuKienSunOne.ModelViews
{
    public class CartItem
    {
        public Product product { get; set; }
        public int amount { get; set; }
        public int totalMoney => product.Discount > 0 ? amount * product.Discount.Value : amount * product.Price.Value;
    }
}
