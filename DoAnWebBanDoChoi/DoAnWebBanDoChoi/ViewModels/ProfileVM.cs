using DoAnWebBanDoChoi.Models;
using System.ComponentModel.DataAnnotations;

namespace DoAnWebBanDoChoi.ViewModels
{
    public class ProfileVM
    {
        [Display(Name = "Họ tên hiển thị")]
        [Required(ErrorMessage = "Vui lòng nhập họ tên hiển thị.")]
        public string? HoTenHienThi { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập Email.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; }

        // ✅ THÊM [Required] và [Display]
        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải gồm 10 chữ số và bắt đầu bằng 0")]
        public string? DienThoai { get; set; }

        // ✅ THÊM [Required] và [Display]
        [Display(Name = "Địa chỉ")]
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ.")]
        public string? DiaChi { get; set; }
        public string? HinhAnh { get; set; }

        public List<DonHang>? DonHangs { get; set; }
        public IFormFile? HinhAnhFile { get; set; }
    }
}
