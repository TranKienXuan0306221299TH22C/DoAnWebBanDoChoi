using DoAnWebBanDoChoi.Helpers;
using DoAnWebBanDoChoi.Models;
using DoAnWebBanDoChoi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

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
                    .ToList(),
                
            };
           
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiAnhDaiDien(ProfileVM model)
        {
            // 1. Kiểm tra file và người dùng
            if (model.HinhAnhFile == null)
            {
                TempData["Error"] = "Vui lòng chọn một file ảnh.";
                return RedirectToAction("Index");
            }

            var maNd = HttpContext.Session.Get<int>("MaNd");
            var nguoiDung = await _context.NguoiDungs.FindAsync(maNd);
            var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif" };

            if (!allowedMimeTypes.Contains(model.HinhAnhFile.ContentType.ToLowerInvariant()))
            {
                TempData["Error"] = "File không phải là định dạng ảnh hợp lệ (chỉ chấp nhận JPEG, PNG, GIF).";
                return RedirectToAction("Index");
            }
            if (nguoiDung == null)
                return NotFound();

            // 2. Xử lý lưu file vật lý (Sử dụng cách đơn giản của bạn)
            string? fileName = null;
            try
            {
                // Xác định thư mục upload ảnh đại diện: [Thư mục gốc]/wwwroot/img
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/profile");

                // Tạo tên file duy nhất
                fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.HinhAnhFile.FileName);
                string filePath = Path.Combine(uploadsFolder, fileName);

                // Lưu file vào thư mục
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.HinhAnhFile.CopyToAsync(stream);
                }

                // 3. Cập nhật DB
                nguoiDung.HinhAnh = fileName;
                await _context.SaveChangesAsync();

                TempData["Success"] = "Đổi ảnh đại diện thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi lưu file: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
        public IActionResult DonHang()
        {
            var maNd = HttpContext.Session.Get<int>("MaNd");

            var vm = new ProfileVM
            {
                DonHangs = _context.DonHangs
                    .Where(d => d.MaNd == maNd)
                    .ToList()
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult HuyDonHang(int id)
        {
            var maNd = HttpContext.Session.Get<int>("MaNd");

            // 1. Phải INCLUDE ChiTietDonHangs để lấy danh sách sản phẩm
            var donHang = _context.DonHangs
                .Include(d => d.ChiTietDonHangs)
                .FirstOrDefault(d => d.MaDh == id && d.MaNd == maNd);

            if (donHang == null)
            {
                TempData["Error"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction("Index");
            }

            // Kiểm tra trạng thái hợp lệ trước khi hủy 
            if (donHang.TrangThai == 5 || donHang.TrangThai == 4)
            {
                TempData["Error"] = "Đơn hàng không thể hủy ở trạng thái này.";
                return RedirectToAction("DonHang");
            }

            // 2. LẶP QUA TẤT CẢ SẢN PHẨM TRONG ĐƠN HÀNG VÀ HOÀN NHẬP TỒN KHO
            foreach (var chiTiet in donHang.ChiTietDonHangs)
            {
                var sp = _context.SanPhams.FirstOrDefault(s => s.MaSp == chiTiet.MaSp);

                if (sp != null)
                {
                    // CỘNG số lượng đã đặt hàng (ct.SoLuong) trở lại vào tồn kho (sp.SoLuong)
                    sp.SoLuong += chiTiet.SoLuong;
                }
            }

            // 3. Cập nhật trạng thái và lưu thay đổi
            donHang.TrangThai = 5; // Đã hủy
            donHang.NgaySua = DateTime.Now;

            _context.SaveChanges(); 

            TempData["Success"] = "Hủy đơn hàng và hoàn nhập tồn kho thành công!";

            return RedirectToAction("DonHang");
        }


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

        public IActionResult ChiTietDonHang(int id)
        {
            var maNd = HttpContext.Session.Get<int>("MaNd");

            var donHang = _context.DonHangs
                .Include(dh => dh.ChiTietDonHangs)
                    .ThenInclude(ct => ct.MaSpNavigation)
                .FirstOrDefault(dh => dh.MaDh == id && dh.MaNd == maNd);

            if (donHang == null)
                return NotFound();

            var chiTietVM = donHang.ChiTietDonHangs.Select(ct => new ChiTietDonHangBinhLuanVM
            {
                MaCtdh = ct.MaCtdh,
                MaSp = ct.MaSp,
                TenSanPham = ct.MaSpNavigation.TenSanPham,
                HinhAnh = ct.MaSpNavigation.HinhAnh,
                SoLuong = ct.SoLuong,
                DonGia = ct.DonGia,
                DaBinhLuan = _context.BinhLuans.Any(b => b.MaNd == maNd && b.MaSp == ct.MaSp && b.MaNd == donHang.MaNd)
            }).ToList();

            ViewBag.DonHang = donHang; // giữ lại thông tin đơn hàng chung
            return View(chiTietVM);
        }


    }
}
