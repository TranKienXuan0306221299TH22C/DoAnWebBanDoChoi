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

        public IActionResult Index(int? loai, string? sort, int page = 1, int pageSize = 6)
        {
            var query = _context.SanPhams
                .Include(sp => sp.MaDmNavigation)
                .Where(sp => !loai.HasValue || sp.MaDm == loai.Value)
                .OrderByDescending(sp => sp.MaSp);
            switch (sort)
            {
                case "new":
                    query = query.OrderByDescending(sp => sp.NgayTao);
                    break;

                case "price-asc":
                    query = query.OrderBy(sp => sp.DonGia);
                    break;

                case "price-desc":
                    query = query.OrderByDescending(sp => sp.DonGia);
                    break;

                case "hot":
                    // Giả sử bán chạy dựa vào số lượng bán (phải tính từ ChiTietDonHang nếu có)
                    query = query.OrderByDescending(sp => sp.ChiTietDonHangs.Count);
                    break;
                case "favorite":
                    query = query
                        .Where(sp => sp.SanPhamYeuThiches.Any()) // chỉ lấy sản phẩm có yêu thích
                        .OrderByDescending(sp => sp.SanPhamYeuThiches.Count); // sắp theo số lượt
                    break;
                default:
                    query = query.OrderByDescending(sp => sp.MaSp); // Mặc định
                    break;
            }
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


        //public IActionResult Detail(int id)
        //{
        //    var sanPham = _context.SanPhams
        //        .Include(sp => sp.MaDmNavigation)    
        //        .Include(sp => sp.MaThNavigation)    
        //        .SingleOrDefault(sp => sp.MaSp == id && sp.TrangThai == 1);

        //    if (sanPham == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(sanPham);
        //}
        [Route("san-pham/{slug}-{id}")] 
        public IActionResult Detail(string slug, int id)
        {
            var sanPham = _context.SanPhams
                .Include(sp => sp.MaDmNavigation)
                .Include(sp => sp.MaThNavigation)
                .SingleOrDefault(sp => sp.MaSp == id && sp.TrangThai == 1);

            if (sanPham == null)
                return Redirect("/404");

            // Nếu slug không đúng → redirect đúng (chuẩn SEO)
            if (sanPham.Slug != slug)
            {
                return RedirectToAction("Detail", new { slug = sanPham.Slug, id = sanPham.MaSp });
            }

            return View(sanPham);
        }


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
