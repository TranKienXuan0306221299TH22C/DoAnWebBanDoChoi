using System.ComponentModel.DataAnnotations;

namespace DoAnWebBanDoChoi.ViewModels.Admin
{
    public class NhanVienVM
    {
        [Required(ErrorMessage = "Vui lòng nhập Tên Đăng Nhập.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập phải dài từ 3 đến 50 ký tự.")]
        public string TenDangNhap { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập Mật Khẩu.")]
        [DataType(DataType.Password)]
        public string MatKhau { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập Email.")]
        [EmailAddress(ErrorMessage = "Địa chỉ Email không hợp lệ.")]
        public string Email { get; set; } = null!;

        // 🆕 Thêm Tên Hiển Thị (Có thể không bắt buộc)
        [StringLength(100)]
        public string? HoTenHienThi { get; set; }
    }
}
