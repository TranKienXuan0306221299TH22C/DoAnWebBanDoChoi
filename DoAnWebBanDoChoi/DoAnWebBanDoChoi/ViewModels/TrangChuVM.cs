using DoAnWebBanDoChoi.Models;
using X.PagedList;

namespace DoAnWebBanDoChoi.ViewModels
{
    public class TrangChuVM
    {
        public IPagedList<SanPham> DanhSachSanPham { get; set; }
        public List<SanPham> SanPhamMoi { get; set; }
        public string? Keyword { get; set; }
    }
}
