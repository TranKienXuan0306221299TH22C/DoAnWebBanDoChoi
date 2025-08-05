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

        public IActionResult Index(int? loai, string? sort, int page = 1)
        {
            var query = _context.SanPhams
                .Include(sp => sp.MaDmNavigation)
                .Where(sp => !loai.HasValue || sp.MaDm == loai.Value);
                //.OrderByDescending(sp => sp.MaSp);
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
                case "all":
                    // không sắp xếp gì cả — cứ lấy mặc định từ CSDL
                    break;
                default:
                    query = query.OrderBy(sp => sp.MaSp);  // Mặc định
                    break;
            }
            int pageSize = 6;
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

        //[Route("san-pham/{slug}-{id}")] 
        //public IActionResult Detail(string slug, int id)
        //{
        //    var sanPham = _context.SanPhams
        //        .Include(sp => sp.MaDmNavigation)
        //        .Include(sp => sp.MaThNavigation)
        //        .SingleOrDefault(sp => sp.MaSp == id && sp.TrangThai == 1);
        //    var binhLuan = _context.BinhLuans
        //        .Include(nd => nd.MaNdNavigation)
        //        .Include(sp => sp.MaSpNavigation)
        //        .Where(sp=>sp.MaSp==id)
        //        .ToList();
        //    if (sanPham == null)
        //        return Redirect("/404");

        //    // Nếu slug không đúng → redirect đúng (chuẩn SEO)
        //    if (sanPham.Slug != slug)
        //    {
        //        return RedirectToAction("Detail", new { slug = sanPham.Slug, id = sanPham.MaSp });
        //    }
        //    var model = new ChiTietSanPhamVM
        //    {
        //        SanPham = sanPham,
        //        BinhLuans = binhLuan
        //    };

        //    return View(model);
        //}
        [Route("san-pham/{slug}-{id}")]
        public IActionResult Detail(string slug, int id, int? page)
        {
            var sanPham = _context.SanPhams
                .Include(sp => sp.MaDmNavigation)
                .Include(sp => sp.MaThNavigation)
                .SingleOrDefault(sp => sp.MaSp == id && sp.TrangThai == 1);

            if (sanPham == null)
                return Redirect("/404");

            if (sanPham.Slug != slug)
            {
                return RedirectToAction("Detail", new { slug = sanPham.Slug, id = sanPham.MaSp });
            }

            int pageSize = 3;
            //int pageNumber = page ?? 1;
            int pageNumber = Math.Max(page ?? 1, 1);
            var binhLuan = _context.BinhLuans
                .Include(bl => bl.MaNdNavigation)
                .Include(bl => bl.MaSpNavigation)
                .Where(bl => bl.MaSp == id )
                .OrderByDescending(bl => bl.NgayTao)
                .ToPagedList(pageNumber, pageSize);

            var model = new ChiTietSanPhamVM
            {
                SanPham = sanPham,
                BinhLuans = binhLuan
            };

            return View(model);
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

           
            return View("Index", model);
        }

    }
}
