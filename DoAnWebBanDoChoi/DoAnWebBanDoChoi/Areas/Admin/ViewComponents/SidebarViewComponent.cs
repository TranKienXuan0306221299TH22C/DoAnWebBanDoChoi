using Microsoft.AspNetCore.Mvc;
using DoAnWebBanDoChoi.Models;
using DoAnWebBanDoChoi.Helpers;
using DoAnWebBanDoChoi.ViewModels;

namespace DoAnWebBanDoChoi.Areas.Admin.ViewComponents
{
    public class SidebarViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public SidebarViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            int? maNd = HttpContext.Session.Get<int>("MaNd");

            string hoTen = "";
            string vaiTro = ""; // mặc định nếu không có session

            if (maNd != null)
            {
                var nd = await _context.NguoiDungs.FindAsync(maNd.Value);
                if (nd != null)
                {
                    hoTen = nd.HoTenHienThi ?? nd.TenDangNhap;
                    vaiTro = nd.VaiTro;
                }
            }

            var model = new SidebarVM
            {
                HoTen = hoTen,
                VaiTro = vaiTro
            };

            return View("Sidebar", model);
        }

    }
}
