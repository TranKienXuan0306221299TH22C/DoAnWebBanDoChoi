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
                    HieuLuc = true
                };

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
        [HttpPost]
        public IActionResult Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var hashedPassword = HashHelper.HashPassword(model.MatKhau);

                var user = _context.NguoiDungs
                            .FirstOrDefault(x => x.TenDangNhap == model.TenDangNhap && x.MatKhau == hashedPassword && x.HieuLuc);

                if (user != null)
                {
                    HttpContext.Session.Set("MaNd", user.MaNd);             
                    HttpContext.Session.SetString("DangNhap", "true");
                    return RedirectToAction("Index", "Home");
                }

                // Nếu không có user → báo lỗi
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
