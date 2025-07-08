using DoAnWebBanDoChoi.Helpers;
using DoAnWebBanDoChoi.Models;
using DoAnWebBanDoChoi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAnWebBanDoChoi.Controllers
{
    public class ProfileController : Controller
    {
        private readonly AppDbContext _context;

        public ProfileController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var maNd = HttpContext.Session.Get<int>("MaNd");    
            var nd = _context.NguoiDungs
                .Include(x => x.DonHangs)
                .FirstOrDefault(x => x.MaNd == maNd);

            if (nd == null)
                return NotFound();

            var vm = new ProfileVM
            {
                HoTenHienThi = nd.HoTenHienThi,
                Email = nd.Email,
                DienThoai = nd.DienThoai,
                DiaChi = nd.DiaChi,
                HinhAnh = nd.HinhAnh,
                DonHangs = nd.DonHangs
                    .OrderByDescending(d => d.NgayTao)
                    .ToList()
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult HuyDonHang(int id)
        {
            var maNd = HttpContext.Session.Get<int>("MaNd");
  

            var donHang = _context.DonHangs
                .FirstOrDefault(d => d.MaDh == id && d.MaNd == maNd);

            if (donHang == null)
            {
                TempData["Error"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction("Index");
            }

                donHang.TrangThai = 5; // Đã hủy
                donHang.NgaySua = DateTime.Now;
                _context.SaveChanges();
                TempData["Success"] = "Hủy đơn hàng thành công!";
        

            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Edit()
        {
            var maNd = HttpContext.Session.Get<int>("MaNd");
            var nd = _context.NguoiDungs.Find(maNd);

            if (nd == null) return RedirectToAction("Login", "Account");

            var vm = new ProfileVM
            {
                HoTenHienThi = nd.HoTenHienThi,
                DienThoai = nd.DienThoai,
                DiaChi = nd.DiaChi
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult Edit(ProfileVM model)
        {
            var maNd = HttpContext.Session.Get<int>("MaNd");
            var nd = _context.NguoiDungs.Find(maNd);

            if (nd == null) return RedirectToAction("Login", "Account");

            // Cập nhật dữ liệu
            nd.HoTenHienThi = model.HoTenHienThi;
            nd.DienThoai = model.DienThoai;
            nd.DiaChi = model.DiaChi;
          

            _context.SaveChanges();

            TempData["Success"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("Index");
        }










    }
}
