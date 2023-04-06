using System.ComponentModel.DataAnnotations;

namespace WebsitePhuKienSunOne.ModelViews
{
    public class ChangePasswordVM
    {
        [Key]
        public int CustomerID { get; set; }
        [Display(Name ="Mật khẩu cũ")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu cũ")]
        public string OldPassword { get; set; }
        [Display(Name ="Mật khẩu mới")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
        [MinLength(8, ErrorMessage = "Mật khẩu tối thiểu 8 ký tự")]
        public string NewPassword { get; set; }
        [Display(Name = "Mật khẩu mới")]
        [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu mới")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu không trùng khớp")]
        public string ConfirmPassword { get; set;}
    }
}
