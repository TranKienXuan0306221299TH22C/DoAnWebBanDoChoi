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
                TrangThaiDonHang.DaXacNhan => "Xác nhận",
                TrangThaiDonHang.DangGiao => "Vận chuyển",
                TrangThaiDonHang.DaGiao => "Hoàn thành",
                TrangThaiDonHang.DaHuy => "Đã huỷ",
                _ => "Không rõ"
            };
        }
    }
}
