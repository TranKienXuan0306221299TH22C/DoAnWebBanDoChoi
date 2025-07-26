using DoAnWebBanDoChoi.Filters;
using DoAnWebBanDoChoi.Models;
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
        public IActionResult NhanVien()
        {
            var tk = _context.NguoiDungs.
                Where(tk => tk.VaiTro == "nhanvien").ToList();
            return View(tk);
        }
    }
}
