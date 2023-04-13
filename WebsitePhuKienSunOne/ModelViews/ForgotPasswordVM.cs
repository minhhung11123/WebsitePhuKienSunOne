using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace WebsitePhuKienSunOne.ModelViews
{
    public class ForgotPasswordVM
    {
        [Key]
        [MaxLength(100)]
        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [Display(Name = "Địa chỉ Email")]
        public string Email { get; set; }
    }
}
