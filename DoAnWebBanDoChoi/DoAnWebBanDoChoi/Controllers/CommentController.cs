using DoAnWebBanDoChoi.Helpers;
using DoAnWebBanDoChoi.Models;
using DoAnWebBanDoChoi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAnWebBanDoChoi.Controllers
{
    public class CommentController : Controller
    {
        private readonly AppDbContext _context;

        public CommentController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult BinhLuan()
        {
            var maNd = HttpContext.Session.Get<int>("MaNd");

            var sanPhams = _context.ChiTietDonHangs
                .Where(ct => ct.MaDhNavigation.MaNd == maNd && ct.MaDhNavigation.TrangThai == 4)
                .Include(ct => ct.MaSpNavigation)
                .ToList()
                .GroupBy(ct => ct.MaSp) // nhóm lại tránh trùng sản phẩm
                .Select(g =>
                {
                    var sp = g.First().MaSpNavigation;
                    var daBinhLuan = _context.BinhLuans.Any(b => b.MaSp == sp.MaSp && b.MaNd == maNd);
                    return new SanPhamDaMuaVM
                    {
                        MaSp = sp.MaSp,
                        TenSanPham = sp.TenSanPham,
                        HinhAnh = sp.HinhAnh,
                        DonGia = sp.DonGia,
                        DaBinhLuan = daBinhLuan
                    };
                })
                .ToList();

            return View(sanPhams);
        }

        public IActionResult ThemBinhLuan(int maSp)
        {
            var maNd = HttpContext.Session.Get<int>("MaNd");

            // Kiểm tra đã bình luận chưa
            var daBinhLuan = _context.BinhLuans.Any(b => b.MaNd == maNd && b.MaSp == maSp);
            if (daBinhLuan)
            {
                return RedirectToAction("BinhLuan");
            }

            var sp = _context.SanPhams.FirstOrDefault(s => s.MaSp == maSp);
            if (sp == null) return NotFound();

            var vm = new BinhLuanVM
            {
                MaSp = sp.MaSp,
                TenSanPham = sp.TenSanPham,
                HinhAnh = sp.HinhAnh
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult ThemBinhLuan(BinhLuanVM model)
        {
            var maNd = HttpContext.Session.Get<int>("MaNd");
            if (!ModelState.IsValid)
            {
                // Đổ lại tên SP & hình ảnh nếu validation lỗi
                var sp = _context.SanPhams.Find(model.MaSp);
                model.TenSanPham = sp?.TenSanPham ?? "";
                model.HinhAnh = sp?.HinhAnh;
                return View(model);
            }

            var binhLuan = new BinhLuan
            {
                MaSp = model.MaSp,
                MaNd = maNd,
                Diem = model.SoSao,
                NoiDungBinhLuan = model.NoiDung,
                TrangThai = 0,
                NgayTao = DateTime.Now
            };

            _context.BinhLuans.Add(binhLuan);
            _context.SaveChanges();

            return RedirectToAction("BinhLuan");
        }


    }
}
