using DoAnWebBanDoChoi.Filters;
using DoAnWebBanDoChoi.Helpers;
using DoAnWebBanDoChoi.Models;
using DoAnWebBanDoChoi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAnWebBanDoChoi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class CommentController : Controller
    {
        private readonly AppDbContext _context;

        public CommentController(AppDbContext context)
        {
            _context = context;
        }


        public IActionResult QuanLyBinhLuan()
        {
            var maNd = HttpContext.Session.Get<int>("MaNd");

            var vm = new QuanLyBinhLuanVM
            {
                BinhLuanChuaDuyet = LayDanhSachBinhLuan(maNd, 0),
                BinhLuanDaDuyet = LayDanhSachBinhLuan(maNd, 1)
            };

            return View(vm);
        }

        private List<SanPhamDaMuaVM> LayDanhSachBinhLuan(int maNd, int trangThai)
        {
            return _context.BinhLuans
                .Where(b => b.MaNd == maNd && b.TrangThai == trangThai)
                .Include(b => b.MaSpNavigation)
                .Select(b => new SanPhamDaMuaVM
                {
                    MaSp = b.MaSp,
                    TenSanPham = b.MaSpNavigation.TenSanPham,
                    HinhAnh = b.MaSpNavigation.HinhAnh,
                    DonGia = b.MaSpNavigation.DonGia,
                    DaBinhLuan = true
                })
                .ToList();
        }
    }
}
