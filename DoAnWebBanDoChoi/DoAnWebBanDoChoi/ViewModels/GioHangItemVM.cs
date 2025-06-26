namespace DoAnWebBanDoChoi.ViewModels
{
    public class GioHangItemVM
    {
        public int MaGh { get; set; }
        public int MaSp { get; set; }
        public string TenSp { get; set; } = string.Empty;
        public string HinhAnh { get; set; } = string.Empty;
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }

        // Tính tổng tiền mỗi dòng
        public decimal TongTien => SoLuong * DonGia;
    }
}
