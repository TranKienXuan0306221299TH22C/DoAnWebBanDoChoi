using DoAnWebBanDoChoi.Models;
using System.ComponentModel.DataAnnotations;

namespace DoAnWebBanDoChoi.ViewModels
{
    public class ProfileVM
    {
        public string? HoTenHienThi { get; set; }
        public string Email { get; set; }
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải gồm 10 chữ số và bắt đầu bằng 0")]
        public string? DienThoai { get; set; }
        public string? DiaChi { get; set; }
        public string? HinhAnh { get; set; }

        public List<DonHang>? DonHangs { get; set; }
    }
}
