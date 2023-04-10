using System.ComponentModel.DataAnnotations;

namespace WebsitePhuKienSunOne.ModelViews
{
    public class CheckoutVM
    {
        public int CustomerId { get; set; }
        [Required(ErrorMessage ="Vui lòng nhập họ tên")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Nhập địa chỉ nhận hàng")]
        public string Address { get; set; }
        [Range(1,int.MaxValue,ErrorMessage = "Vui lòng chọn Tỉnh/Thành")]
        public int City { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn Quận/Huyện")]
        public int District { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn Phường/Xã")]
        public int Ward { get; set; }
        public int PaymentID { get; set; }
        public string Note { get; set; }
    }
}
