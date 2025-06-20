using DoAnWebBanDoChoi.Models;
using DoAnWebBanDoChoi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace DoAnWebBanDoChoi.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        [Route("/404")]
        public IActionResult Loi()
        {
            return View();
        }


        public IActionResult Index(int? loai, int page = 1, int pageSize = 1)
        {
            var query = _context.SanPhams
                    .Include(sp => sp.MaDmNavigation)
                    .Where(sp => !loai.HasValue || sp.MaDm == loai.Value)
                    .OrderByDescending(sp => sp.MaSp);
            var danhSach = query.ToPagedList(page, pageSize);

            var cauHinh = _context.CauHinhs.FirstOrDefault();

            var vm = new HomeVM
            {
                SanPhams = danhSach,
                CauHinh = cauHinh!
            };

            return View(vm);
        }

        public IActionResult Detail(int id)
        {
            var sanPham = _context.SanPhams
                .Include(sp => sp.MaDmNavigation)    
                .Include(sp => sp.MaThNavigation)    
                .SingleOrDefault(sp => sp.MaSp == id && sp.TrangThai == 1);

            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }

    }
}
