using X.PagedList;
using DoAnWebBanDoChoi.Models;

namespace DoAnWebBanDoChoi.ViewModels
{
    public class ChiTietSanPhamVM
    {
        public SanPham SanPham { get; set; }
        
        public IPagedList<BinhLuan> BinhLuans { get; set; }
        public List<SanPham> SanPhamLienQuan { get; set; }
        public double DiemSaoTrungBinh { get; set; }
    }
}
