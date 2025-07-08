using DoAnWebBanDoChoi.Enums;

namespace DoAnWebBanDoChoi.Helpers
{
    public static class TrangThaiDonHangExtensions
    {
        public static string GetDisplayName(this TrangThaiDonHang trangThai)
        {
            return trangThai switch
            {
                TrangThaiDonHang.ChoXacNhan => "Chờ xác nhận",
                TrangThaiDonHang.DaXacNhan => "Đã xác nhận",
                TrangThaiDonHang.DangGiao => "Đang giao",
                TrangThaiDonHang.DaGiao => "Đã giao",
                TrangThaiDonHang.DaHuy => "Đã huỷ",
                _ => "Không rõ"
            };
        }
    }
}
