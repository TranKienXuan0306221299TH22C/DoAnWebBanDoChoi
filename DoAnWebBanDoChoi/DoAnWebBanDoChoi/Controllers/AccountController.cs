using DoAnWebBanDoChoi.Helpers;
using DoAnWebBanDoChoi.Models;
using DoAnWebBanDoChoi.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DoAnWebBanDoChoi.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public IActionResult Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                var check = _context.NguoiDungs.FirstOrDefault(x =>
                    x.TenDangNhap == model.TenDangNhap || x.Email == model.Email);

                if (check != null)
                {
                    ModelState.AddModelError("", "Tên đăng nhập hoặc email đã tồn tại.");
                    return View(model);
                }

                var nguoiDung = new NguoiDung
                {
                    TenDangNhap = model.TenDangNhap,
                    Email = model.Email,
                    MatKhau = HashHelper.HashPassword(model.MatKhau),
                    VaiTro = "user",
                    HieuLuc = true,
                    HinhAnh = "user.png"
                };
                TempData["Success"] = "Đăng ký thành công! Bạn có thể đăng nhập ngay.";
                _context.NguoiDungs.Add(nguoiDung);
                _context.SaveChanges();
               
                return RedirectToAction("Login");
            }


            return View(model);
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var hashedPassword = HashHelper.HashPassword(model.MatKhau);

                // BƯỚC 1: Tìm người dùng chỉ dựa trên Tên/Email và Mật khẩu (KHÔNG kiểm tra HieuLuc ngay)
                var user = _context.NguoiDungs
                    .FirstOrDefault(x =>
                        (x.TenDangNhap == model.TenDangNhap || x.Email == model.TenDangNhap)
                        && x.MatKhau == hashedPassword);

                if (user != null)
                {
                    // BƯỚC 2: KIỂM TRA TRẠNG THÁI KHÓA (HieuLuc)
                    if (user.HieuLuc == false)
                    {
                        // Tài khoản bị khóa, hiển thị thông báo CỤ THỂ
                        ModelState.AddModelError("", "Tài khoản của bạn đã bị vô hiệu hóa. Vui lòng liên hệ bộ phận hỗ trợ.");
                        return View(model);
                    }

                    // Nếu HieuLuc == true: ĐĂNG NHẬP THÀNH CÔNG
                    HttpContext.Session.Set("MaNd", user.MaNd);
                    HttpContext.Session.SetString("DangNhap", "true");
                    HttpContext.Session.SetString("VaiTro", user.VaiTro);
                    HttpContext.Session.SetString("HoTen", user.TenDangNhap);

                    if (user.VaiTro == "admin" || user.VaiTro == "nhanvien")
                    {
                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                // Nếu không tìm thấy user ở BƯỚC 1 (Sai tên đăng nhập/email hoặc mật khẩu) → báo lỗi chung
                ModelState.AddModelError("", "Sai tên đăng nhập hoặc mật khẩu.");
            }

            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }

}
