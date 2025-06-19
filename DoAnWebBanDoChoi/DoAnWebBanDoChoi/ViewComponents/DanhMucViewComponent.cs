using DoAnWebBanDoChoi.Models;
using Microsoft.AspNetCore.Mvc;
using DoAnWebBanDoChoi.ViewModels;

namespace DoAnWebBanDoChoi.ViewComponents
{
    public class DanhMucViewComponent : ViewComponent
    {
        private readonly AppDbContext db;

        public DanhMucViewComponent(AppDbContext context) => db = context;

        public IViewComponentResult Invoke()
        {
            var data = db.DanhMucs.Select(dm => new DanhMucVM
            {
               MaDanhMuc= dm.MaDm,
               TenDanhMuc= dm.TenDanhMuc
            }).OrderBy(p=>p.TenDanhMuc);
            return View(data);
        }
    }
}
