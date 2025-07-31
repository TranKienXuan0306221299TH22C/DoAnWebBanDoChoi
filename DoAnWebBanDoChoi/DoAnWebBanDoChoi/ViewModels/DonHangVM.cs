using System;
using DoAnWebBanDoChoi.Enums;
using DoAnWebBanDoChoi.Helpers;

namespace DoAnWebBanDoChoi.ViewModels
{
    public class DonHangVM
    {
        public int MaDh { get; set; }
        public string? HoTen { get; set; }
        public string? DienThoai { get; set; }
        public string? DiaChiDayDu { get; set; }
        public DateTime NgayTao { get; set; }
        public decimal TongTien { get; set; }
        public string? PhuongThucThanhToan { get; set; }
        public TrangThaiDonHang TrangThai { get; set; }
        public string TrangThaiHienThi => TrangThai.GetDisplayName();
    }
}
