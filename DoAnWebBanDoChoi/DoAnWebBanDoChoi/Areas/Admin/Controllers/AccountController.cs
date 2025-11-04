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
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult KhachHang()
        {
            var tk = _context.NguoiDungs.
                Where(tk => tk.VaiTro == "user").ToList();
            return View(tk);
        }
        
        [HttpPost] // Nên dùng POST để thay đổi dữ liệu
        public IActionResult ToggleHieuLuc(int id)
        {
            var nguoiDung = _context.NguoiDungs.SingleOrDefault(nd => nd.MaNd == id);

            if (nguoiDung == null)
            {
                // Trả về lỗi 404 nếu không tìm thấy người dùng
                return NotFound();
            }

            // Đảo ngược trạng thái hiện tại (true thành false, false thành true)
            nguoiDung.HieuLuc = !nguoiDung.HieuLuc;

            _context.NguoiDungs.Update(nguoiDung);
            _context.SaveChanges();

            // Quay lại trang danh sách khách hàng sau khi cập nhật
            // Bạn có thể dùng TempData để thông báo thành công.
            TempData["Success"] = $"Đã cập nhật trạng thái tài khoản {nguoiDung.TenDangNhap}.";
            return RedirectToAction("KhachHang");
        }
        public IActionResult NhanVien()
        {
            var tk = _context.NguoiDungs.
                Where(tk => tk.VaiTro == "nhanvien").ToList();
            return View(tk);
        }
        public IActionResult Create()
        {
            return View();
        }

        // POST: Xử lý Thêm tài khoản nhân viên
        [HttpPost]
        public IActionResult Create(NhanVienVM model)
        {
            if (ModelState.IsValid)
            {
                var check = _context.NguoiDungs.FirstOrDefault(x => x.TenDangNhap == model.TenDangNhap);

                if (check != null)
                {
                    ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã tồn tại.");
                    return View(model);
                }

                var nguoiDung = new NguoiDung
                {
                    TenDangNhap = model.TenDangNhap,
                    MatKhau = HashHelper.HashPassword(model.MatKhau), 
                    VaiTro = "nhanvien",
                    Email = model.Email, 
                    HoTenHienThi = model.HoTenHienThi,
                    HieuLuc = true,
                    HinhAnh = "nhanvien.png" 
                };

                _context.NguoiDungs.Add(nguoiDung);
                _context.SaveChanges();

                TempData["Success"] = $"Đã thêm tài khoản nhân viên {model.TenDangNhap} thành công.";
                return RedirectToAction("NhanVien"); // Quay lại danh sách nhân viên
            }

            return View(model);
        }

        // POST: Bật/Tắt Trạng Thái Tài Khoản Nhân Viên
        [HttpPost]
        public IActionResult ToggleHieuLucNhanVien(int id)
        {
            var nguoiDung = _context.NguoiDungs
                .Where(nd => nd.VaiTro == "nhanvien") // Chỉ tìm trong nhân viên
                .SingleOrDefault(nd => nd.MaNd == id);

            if (nguoiDung == null)
            {
                return NotFound();
            }

            // Đảo ngược trạng thái hiện tại (true thành false, false thành true)
            nguoiDung.HieuLuc = !nguoiDung.HieuLuc;

            _context.NguoiDungs.Update(nguoiDung);
            _context.SaveChanges();

            TempData["SuccessMessage"] = $"Đã cập nhật trạng thái tài khoản {nguoiDung.TenDangNhap}.";
            return RedirectToAction("NhanVien");
        }
    }
}
