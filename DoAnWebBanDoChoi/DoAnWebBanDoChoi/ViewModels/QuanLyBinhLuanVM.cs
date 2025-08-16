namespace DoAnWebBanDoChoi.ViewModels
{
    public class QuanLyBinhLuanVM
    {
        public List<BinhLuanPhanHoiVM> BinhLuanChuaDuyet { get; set; } = new();
        public List<BinhLuanPhanHoiVM> BinhLuanDaDuyet { get; set; } = new();
        public List<BinhLuanPhanHoiVM> BinhLuanDaAn { get; set; } = new();
    }
}
