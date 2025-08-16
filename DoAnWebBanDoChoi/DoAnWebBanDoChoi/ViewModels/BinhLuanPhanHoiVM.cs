namespace DoAnWebBanDoChoi.ViewModels
{
    public class BinhLuanPhanHoiVM
    {
        public int MaBl { get; set; }
        public string TenSanPham { get; set; } = "";
        public string TenNguoiDung { get; set; } = "";
        public int Diem { get; set; }
        public string NoiDung { get; set; } = "";
        public string? PhanHoi { get; set; }
        public int TrangThai { get; set; }
        public DateTime NgayTao { get; set; }
    }
}
