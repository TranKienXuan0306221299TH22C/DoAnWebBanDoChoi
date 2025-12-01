using DoAnWebBanDoChoi.Filters;
using DoAnWebBanDoChoi.Helpers;
using DoAnWebBanDoChoi.Models;
using DoAnWebBanDoChoi.ViewModels.Admin;
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
            var vm = new QuanLyBinhLuanVM
            {
                BinhLuanChuaDuyet = LayDanhSachBinhLuan(0),
                BinhLuanDaDuyet = LayDanhSachBinhLuan(1),
                BinhLuanDaAn = LayDanhSachBinhLuan(2)
            };

            return View(vm);
        }

        private List<BinhLuanPhanHoiVM> LayDanhSachBinhLuan(int trangThai)
        {
            return _context.BinhLuans
                .Where(b => b.TrangThai == trangThai)
                .Include(b => b.MaSpNavigation)
                .Include(b => b.MaNdNavigation)
                .OrderByDescending(b => b.NgayTao)
                .Select(b => new BinhLuanPhanHoiVM
                {
                    MaBl = b.MaBl,
                    MaSP=b.MaSp,
                    TenSanPham = b.MaSpNavigation.TenSanPham,
                    Slug=b.MaSpNavigation.Slug,
                    TenNguoiDung = b.MaNdNavigation.HoTenHienThi ?? b.MaNdNavigation.TenDangNhap, // tùy tên trường
                    Diem = b.Diem,
                    NoiDung = b.NoiDungBinhLuan,
                    PhanHoi = b.PhanHoiBinhLuan,
                    TrangThai = b.TrangThai,
                    NgayTao = b.NgayTao
                })
                .ToList();
        }

        [HttpPost]
        public IActionResult Duyet(int id)
        {
            var bl = _context.BinhLuans.Find(id);
            if (bl == null) return NotFound();
            bl.TrangThai = 1;
            _context.SaveChanges();
            return RedirectToAction("QuanLyBinhLuan");
        }

        [HttpPost]
        public IActionResult An(int id)
        {
            var bl = _context.BinhLuans.Find(id);
            if (bl == null) return NotFound();
            bl.TrangThai = 2;
            _context.SaveChanges();
            return RedirectToAction("QuanLyBinhLuan");
        }

        [HttpPost]
        public IActionResult PhanHoi(int id, string noiDung)
        {
            var bl = _context.BinhLuans.Find(id);
            if (bl == null) return NotFound();
            if (!string.IsNullOrEmpty(bl.PhanHoiBinhLuan))
            {
                TempData["Error"] = "Bình luận này đã được phản hồi.";
                return RedirectToAction("QuanLyBinhLuan");
            }
            bl.PhanHoiBinhLuan = noiDung;
            _context.SaveChanges();
            return RedirectToAction("QuanLyBinhLuan");
        }
    }

}
