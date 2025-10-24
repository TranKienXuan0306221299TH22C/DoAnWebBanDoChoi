namespace DoAnWebBanDoChoi.ViewModels
{
    public class DashboardVM
    {
        public decimal TongDoanhThu { get; set; }
        public decimal TongLoiNhuan { get; set; }
        public int TongDonHang { get; set; }
        public int TongKhachHang { get; set; }
        public int? Thang { get; set; }
        public int? Nam { get; set; }

        public int MinYear { get; set; }
        public int MaxYear { get; set; }
    }
}
