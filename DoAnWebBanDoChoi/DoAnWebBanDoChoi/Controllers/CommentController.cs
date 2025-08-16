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

   
        public IActionResult ThemBinhLuan(int maSp, int maCtdh, int maDh)
        {
            var sanPham = _context.SanPhams
                .Where(sp => sp.MaSp == maSp)
                .Select(sp => new BinhLuanVM
                {
                    MaSp = sp.MaSp,
                    TenSanPham = sp.TenSanPham,
                    HinhAnh = sp.HinhAnh,
                    MaCtdh = maCtdh,
                    MaDh = maDh
                })
                .FirstOrDefault();

            if (sanPham == null) return NotFound();

            return View(sanPham);
        }


        [HttpPost]
        public IActionResult ThemBinhLuan(BinhLuanVM model)
        {
            var maNd = HttpContext.Session.Get<int>("MaNd");
            if (maNd == 0)
            {
                return RedirectToAction("DangNhap", "Account");
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu bình luận không hợp lệ.";
                return RedirectToAction("ChiTietDonHang", "Profile", new { id = model.MaDh });
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

            TempData["Success"] = "Bình luận thành công.";
            return RedirectToAction("ChiTietDonHang", "Profile", new { id = model.MaDh });
        }

    }
}
