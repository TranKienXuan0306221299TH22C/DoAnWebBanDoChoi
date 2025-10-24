using DoAnWebBanDoChoi.Helpers;
using DoAnWebBanDoChoi.Models;
using Microsoft.AspNetCore.Mvc;
using DoAnWebBanDoChoi.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DoAnWebBanDoChoi.ViewComponents
{
    public class UserInfoViewComponent :  ViewComponent
    {
        private readonly AppDbContext _context;

        public UserInfoViewComponent(AppDbContext context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke()
        {
            // 1. Lấy MaNd từ Session
            var maNd = HttpContext.Session.Get<int>("MaNd");

            // 2. Truy vấn dữ liệu người dùng (Sử dụng FirstOrDefault() thay vì FirstOrDefaultAsync())
            var nguoiDung = _context.NguoiDungs
                          
                           .FirstOrDefault(u => u.MaNd == maNd);
            var viewModel = new InfoHeaderVM
            {
                HinhAnh = nguoiDung?.HinhAnh,
                HoTenHienThi = nguoiDung?.HoTenHienThi,
                HoTenDangNhap = nguoiDung?.TenDangNhap
            };

            return this.View("Default", viewModel);
        }
    }
}
