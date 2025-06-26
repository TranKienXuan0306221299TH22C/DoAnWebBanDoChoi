using DoAnWebBanDoChoi.Helpers;
using DoAnWebBanDoChoi.Models;
using DoAnWebBanDoChoi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAnWebBanDoChoi.Controllers
{
    public class FavoriteController : Controller
    {
        private readonly AppDbContext _context;

        public FavoriteController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Toggle([FromBody] YeuThichVM model) // ✅ Nhận JSON từ body
        {
            var maNd = HttpContext.Session.Get<int>("MaNd");
            Console.WriteLine("✅ Session MaNd: " + maNd);
            Console.WriteLine("✅ Nhận MaSp từ body: " + model.MaSp);

            if (maNd == 0)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để sử dụng chức năng này." });
            }

            var sp = _context.SanPhams.FirstOrDefault(x => x.MaSp == model.MaSp);
            if (sp == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại." });
            }

            var yeuThich = _context.SanPhamYeuThiches
                .FirstOrDefault(x => x.MaNd == maNd && x.MaSp == model.MaSp);

            if (yeuThich != null)
            {
                _context.SanPhamYeuThiches.Remove(yeuThich);
                _context.SaveChanges();
                return Json(new { success = true, status = "removed" });
            }
            else
            {
                var moi = new SanPhamYeuThich
                {
                    MaNd = maNd,
                    MaSp = model.MaSp
                };
                _context.SanPhamYeuThiches.Add(moi);
                _context.SaveChanges();
                return Json(new { success = true, status = "added" });
            }
        }
        public IActionResult DanhSachYeuThich()
        {
            var maNd = HttpContext.Session.Get<int>("MaNd");
            if (maNd == 0)
                return View("XacNhanDangNhap");

            var sanPhams = _context.SanPhamYeuThiches
                .Where(x => x.MaNd == maNd)
                .Include(x => x.MaSpNavigation)
                .Select(x => x.MaSpNavigation)
                .ToList();

            return View(sanPhams);
        }
        [HttpGet]
        public IActionResult LayTongYeuThich()
        {
            var maNd = HttpContext.Session.Get<int>("MaNd");
            if (maNd == 0)
            {
                return Json(new { success = false, soLuong = 0 });
            }

            int tong = _context.SanPhamYeuThiches
                               .Where(x => x.MaNd == maNd)
                               .Count();

            return Json(new { success = true, soLuong = tong });
        }
        [HttpGet]
        public IActionResult Xoa(int maSp)
        {
            var maNd = HttpContext.Session.Get<int>("MaNd");
            if (maNd == 0)
            {
                return RedirectToAction("Login", "Account");
            }

            var yeuThich = _context.SanPhamYeuThiches
                .FirstOrDefault(x => x.MaNd == maNd && x.MaSp == maSp);

            if (yeuThich != null)
            {
                _context.SanPhamYeuThiches.Remove(yeuThich);
                _context.SaveChanges();
            }

            return RedirectToAction("DanhSachYeuThich"); // Quay lại danh sách yêu thích
        }
    }
}
