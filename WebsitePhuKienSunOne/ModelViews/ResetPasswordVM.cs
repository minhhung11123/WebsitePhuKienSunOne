using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace WebsitePhuKienSunOne.ModelViews
{
    public class ResetPasswordVM
    {
        [Key]
        public int CustomerID { get; set; }
        public string Token { get; set; }
        [Display(Name = "Mật khẩu mới")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
        [MinLength(8, ErrorMessage = "Mật khẩu tối thiểu 8 ký tự")]
        public string Password { get; set; }
    }
}
