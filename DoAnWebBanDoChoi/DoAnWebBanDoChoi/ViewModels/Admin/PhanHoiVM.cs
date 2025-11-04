namespace DoAnWebBanDoChoi.ViewModels.Admin
{
    public class PhanHoiVM
    {
        public int MaPhanHoi { get; set; }
        public string HoTen { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string SoDienThoai { get; set; } = null!;
        public string NoiDung { get; set; } = null!;
        public DateTime ThoiGianGui { get; set; }
        public bool IsRead { get; set; }
    }
}
