using System.ComponentModel.DataAnnotations;

namespace WebsitePhuKienSunOne.ModelViews
{
    public class LoginVM
    {
        [Key]
        [MaxLength(100)]
        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [Display(Name = "Địa chỉ Email")]
        public string Email { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [MinLength(8, ErrorMessage = "Mật khẩu tối thiểu 8 ký tự")]
        public string Password { get; set; }
    }
}
