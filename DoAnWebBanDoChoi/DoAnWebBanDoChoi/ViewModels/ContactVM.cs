using System.ComponentModel.DataAnnotations;

namespace DoAnWebBanDoChoi.ViewModels
{
    public class ContactVM
    {
       

        [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
        [Display(Name = "Họ và tên")]
        [StringLength(100)]
        public string HoTen { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        [StringLength(100)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [Display(Name = "Số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        [StringLength(20)]
        public string SoDienThoai { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung liên hệ.")]
        [Display(Name = "Nội dung liên hệ")]
        // Không giới hạn độ dài cho nội dung (giả định cột NộiDung trong DB là NVARCHAR(MAX))
        public string NoiDung { get; set; }
    }
}
