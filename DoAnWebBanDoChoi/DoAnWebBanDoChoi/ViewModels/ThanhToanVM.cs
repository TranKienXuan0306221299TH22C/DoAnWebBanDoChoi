using System.ComponentModel.DataAnnotations;

namespace DoAnWebBanDoChoi.ViewModels
{
    public class ThanhToanVM
    {
        [Required(ErrorMessage = "Họ tên không được bỏ trống")]
        public string HoTen { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được bỏ trống")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Số điện thoại chỉ được chứa chữ số")]
        public string DienThoai { get; set; }

        [Required(ErrorMessage = "Địa chỉ không được bỏ trống")]
        public string DiaChi { get; set; }

        public string GhiChu { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
        public string PhuongThucThanhToan { get; set; }

        public decimal TongTien { get; set; }  // Tổng hàng chưa ship
        public decimal TongTienSauPhiShip { get; set; } // ✅ Tổng đã có ship

        public List<GioHangItemVM> DanhSachSanPham { get; set; } = new();
    }
}
