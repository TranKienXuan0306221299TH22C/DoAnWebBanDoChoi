
using DoAnWebBanDoChoi.Models;
using X.PagedList;

namespace DoAnWebBanDoChoi.ViewModels
{
    public class HomeVM
    {
        public IPagedList<SanPham> SanPhams { get; set; }
        public CauHinh CauHinh { get; set; }
    }
}
