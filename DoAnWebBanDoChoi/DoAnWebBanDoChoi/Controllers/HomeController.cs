using DoAnWebBanDoChoi.Helpers;
using DoAnWebBanDoChoi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;
using DoAnWebBanDoChoi.ViewModels;
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


        //public IActionResult Index(int? loai, int page = 1, int pageSize = 4)
        //{
        //    var query = _context.SanPhams
        //            .Include(sp => sp.MaDmNavigation)
        //            .Where(sp => !loai.HasValue || sp.MaDm == loai.Value)
        //            .OrderByDescending(sp => sp.MaSp);
        //    var danhSach = query.ToPagedList(page, pageSize);
        //    return View(danhSach);
        //}
        public IActionResult Index(int? loai, int page = 1, int pageSize = 4)
        {
            var query = _context.SanPhams
                .Include(sp => sp.MaDmNavigation)
                .Where(sp => !loai.HasValue || sp.MaDm == loai.Value)
                .OrderByDescending(sp => sp.MaSp);

            var danhSach = query.ToPagedList(page, pageSize);

            var sanPhamMoi = _context.SanPhams
                .OrderByDescending(sp => sp.NgayTao)
                .Take(6)
                .ToList();

            var model = new TrangChuVM
            {
                DanhSachSanPham = danhSach,
                SanPhamMoi = sanPhamMoi
            };

            return View(model);
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

        //public IActionResult Search(string keyword, int page = 1, int pageSize = 8)
        //{
        //    if (string.IsNullOrWhiteSpace(keyword))
        //    {
        //        return RedirectToAction("Index");
        //    }

        //    var keywordUnsign = StringHelper.ToUnsign(keyword).ToLower();
        //    var keywordSigned = keyword.ToLower();

        //    var query = _context.SanPhams
        //        //.Include(sp => sp.MaDmNavigation)
        //        .Where(sp => sp.TrangThai == 1)
        //        .AsEnumerable() // 🔥 chuyển sang xử lý bằng LINQ C# để dùng được hàm tự tạo
        //        .Where(sp =>
        //            sp.TenSanPham.ToLower().Contains(keywordSigned) ||
        //            StringHelper.ToUnsign(sp.TenSanPham).ToLower().Contains(keywordUnsign)
        //        )
        //        .OrderByDescending(sp => sp.MaSp);

        //    var danhSach = query.ToPagedList(page, pageSize);

        //    //ViewBag.TuKhoa = keyword;
        //    return View("Index", danhSach);
        //}
        public IActionResult Search(string keyword, int page = 1, int pageSize = 8)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return RedirectToAction("Index");
            }

            var keywordUnsign = StringHelper.ToUnsign(keyword).ToLower();
            var keywordSigned = keyword.ToLower();

            var query = _context.SanPhams
                .Where(sp => sp.TrangThai == 1)
                .AsEnumerable()
                .Where(sp =>
                    sp.TenSanPham.ToLower().Contains(keywordSigned) ||
                    StringHelper.ToUnsign(sp.TenSanPham).ToLower().Contains(keywordUnsign)
                )
                .OrderByDescending(sp => sp.MaSp);

            var danhSach = query.ToPagedList(page, pageSize);

            var sanPhamMoi = _context.SanPhams
                .OrderByDescending(sp => sp.NgayTao)
                .Take(6)
                .ToList();

            var model = new TrangChuVM
            {
                DanhSachSanPham = danhSach,
                SanPhamMoi = sanPhamMoi
            };

            // ViewBag.TuKhoa = keyword; // có thể dùng để highlight
            return View("Index", model);
        }

    }
}
