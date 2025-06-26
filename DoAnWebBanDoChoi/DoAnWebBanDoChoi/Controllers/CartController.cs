using DoAnWebBanDoChoi.Helpers;
using DoAnWebBanDoChoi.Models;
using Microsoft.AspNetCore.Mvc;
using DoAnWebBanDoChoi.ViewModels;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace DoAnWebBanDoChoi.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult ThemVaoGio(int maSp, int soLuong)
        {
            var maNd = HttpContext.Session.Get<int>("MaNd");
            if (maNd == 0)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để mua hàng!" });
            }

            var gioHang = _context.GioHangs
                .FirstOrDefault(g => g.MaNd == maNd && g.MaSp == maSp);

            if (gioHang != null)
            {
                gioHang.SoLuong += soLuong;
                _context.Update(gioHang);
            }
            else
            {
                var newItem = new GioHang
                {
                    MaNd = maNd,
                    MaSp = maSp,
                    SoLuong = soLuong
                };
                _context.GioHangs.Add(newItem);
            }

            _context.SaveChanges();

            // Tính lại số sản phẩm trong giỏ hàng
            int tong = _context.GioHangs
                               .Where(g => g.MaNd == maNd)
                               .Count();

            return Json(new { success = true, soLuong = tong });
        }
        [HttpGet]
        public IActionResult ChiTietGioHang()
        {
            var maNd = HttpContext.Session.Get<int>("MaNd");

            if (maNd == 0)
            {
                //return RedirectToAction("Login", "Account");
                return View("XacNhanDangNhap");
            }

            var gioHangItems = _context.GioHangs
                .Where(g => g.MaNd == maNd)
                .Select(g => new GioHangItemVM
                {
                    MaGh = g.MaGh,
                    MaSp = g.MaSp,
                    TenSp = g.MaSpNavigation.TenSanPham,
                    HinhAnh = g.MaSpNavigation.HinhAnh,
                    SoLuong = g.SoLuong,
                    DonGia = g.MaSpNavigation.DonGia
                })
                .ToList();

            return View(gioHangItems);
        }
        [HttpGet]
        public IActionResult KiemTraDangNhap()
        {
            var daDangNhap = HttpContext.Session.GetString("DangNhap") == "true";
            return Json(new { daDangNhap });
        }
        [HttpGet]
        public IActionResult LayTongSanPham()
        {
            var maNd = HttpContext.Session.Get<int>("MaNd");
            if (maNd == 0)
            {
                return Json(new { success = false, soLuong = 0 });
            }

            // Đếm số sản phẩm khác nhau
            int tong = _context.GioHangs
                               .Where(g => g.MaNd == maNd)
                               .Count();

            return Json(new { success = true, soLuong = tong });
        }

        [HttpPost]
        public IActionResult CapNhatSoLuong([FromBody] CapNhatSoLuongVM model)
        {
            var userId = HttpContext.Session.Get<int>("MaNd");
            if (userId == 0)
            {
                return Json(new { success = false });
            }

            var gioHang = _context.GioHangs
                .Include(g => g.MaSpNavigation) // Load giá từ bảng SanPham
                .FirstOrDefault(g => g.MaNd == userId && g.MaSp == model.IdSanPham);

            if (gioHang != null)
            {
                gioHang.SoLuong = model.SoLuongMoi;
                _context.SaveChanges();

                // ✅ Lấy đơn giá từ bảng SanPham
                decimal gia = gioHang.MaSpNavigation.DonGia;
                decimal tongTienItem = gioHang.SoLuong * gia;

                // ✅ Tính tổng cộng toàn bộ giỏ hàng
                var tongCong = _context.GioHangs
                    .Where(g => g.MaNd == userId)
                    .Include(g => g.MaSpNavigation)
                    .ToList()
                    .Sum(g => g.SoLuong * g.MaSpNavigation.DonGia);

                return Json(new
                {
                    success = true,
                    tongTienItem = tongTienItem.ToString("N2") + " ₫",
                    tongCong = tongCong.ToString("N2") + " ₫"
                });
            }

            return Json(new { success = false });
        }
        [HttpPost]
        public IActionResult XoaSanPham(int maSp)
        {
            var maNd = HttpContext.Session.Get<int>("MaNd");
            if (maNd == 0)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập!" });
            }

            var item = _context.GioHangs.FirstOrDefault(g => g.MaNd == maNd && g.MaSp == maSp);
            if (item != null)
            {
                _context.GioHangs.Remove(item);
                _context.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Không tìm thấy sản phẩm!" });
        }
        [HttpPost]
        public IActionResult XoaHetGioHang()
        {
            var maNd = HttpContext.Session.Get<int>("MaNd");
            if (maNd == 0)
            {
                return Json(new { success = false });
            }

            var items = _context.GioHangs.Where(g => g.MaNd == maNd).ToList();
            if (items.Any())
            {
                _context.GioHangs.RemoveRange(items);
                _context.SaveChanges();
            }

            return Json(new { success = true });
        }



    }
}

