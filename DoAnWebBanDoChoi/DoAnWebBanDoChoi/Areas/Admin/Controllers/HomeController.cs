using DoAnWebBanDoChoi.Enums;
using DoAnWebBanDoChoi.Filters;
using DoAnWebBanDoChoi.Helpers;
using DoAnWebBanDoChoi.Models;
using DoAnWebBanDoChoi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAnWebBanDoChoi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context )
        {
            _context = context;
        }
        public IActionResult Index(int? thang, int? nam)
        {
            if (!nam.HasValue)
            {
                nam = DateTime.Now.Year;
            }
            var query = _context.DonHangs
                .Include(d => d.ChiTietDonHangs)
                .Where(d => d.TrangThai == (int)TrangThaiDonHang.DaGiao);

            if (thang.HasValue && nam.HasValue)
            {
                query = query.Where(d => d.NgayTao.Month == thang.Value && d.NgayTao.Year == nam.Value);
            }
            else if (thang.HasValue && !nam.HasValue)
            {
                // Nếu chỉ chọn tháng thì mặc định lấy năm hiện tại
                int currentYear = DateTime.Now.Year;
                query = query.Where(d => d.NgayTao.Month == thang.Value && d.NgayTao.Year == currentYear);
            }
            else if (nam.HasValue)
            {
                query = query.Where(d => d.NgayTao.Year == nam.Value);
            }

            var donHangs = query.ToList();

            // 1. Tổng doanh thu
            decimal tongDoanhThu = donHangs.Sum(d => d.TongTien);

            // 2. Tổng lợi nhuận = (Đơn giá - Giá gốc) * Số lượng
            decimal tongLoiNhuan = donHangs.Sum(d =>
                d.ChiTietDonHangs.Sum(ct => (ct.DonGia - ct.GiaGoc) * ct.SoLuong));

            // 3. Tổng đơn hàng
            int tongDonHang = donHangs.Count;

            // 4. Tổng khách hàng (vai trò user)
            int tongKhachHang = _context.NguoiDungs.Count(u => u.VaiTro == "user");

            int minYear = _context.DonHangs
    .Where(d => d.TrangThai == (int)TrangThaiDonHang.DaGiao)
    .Min(d => d.NgayTao.Year);

            int maxYear = _context.DonHangs
                .Where(d => d.TrangThai == (int)TrangThaiDonHang.DaGiao)
                .Max(d => d.NgayTao.Year);

            var model = new DashboardVM
            {
                TongDoanhThu = tongDoanhThu,
                TongLoiNhuan = tongLoiNhuan,
                TongDonHang = tongDonHang,
                TongKhachHang = tongKhachHang,
                Thang = thang,
                Nam = nam,
                MinYear = minYear,
                MaxYear = maxYear
            };

            return View(model);
        }
        // Doanh thu 12 tháng (biểu đồ đường)
        public IActionResult BieuDoDoanhThu(int nam)
        {
            var data = _context.DonHangs
                .Where(d => d.TrangThai == (int)TrangThaiDonHang.DaGiao && d.NgayTao.Year == nam)
                .GroupBy(d => d.NgayTao.Month)
                .Select(g => new
                {
                    Thang = g.Key,
                    DoanhThu = g.Sum(d => d.TongTien)
                })
                .ToList();

            // Đảm bảo đủ 12 tháng
            var result = Enumerable.Range(1, 12)
                .Select(m => new
                {
                    Thang = m,
                    DoanhThu = data.FirstOrDefault(x => x.Thang == m)?.DoanhThu ?? 0
                });

            return Json(result);
        }

        // Doanh thu theo danh mục (biểu đồ cột)
        public IActionResult BieuDoDanhMuc(int nam)
        {
            var data = _context.ChiTietDonHangs
                .Where(ct => ct.MaDhNavigation.TrangThai == (int)TrangThaiDonHang.DaGiao
                             && ct.MaDhNavigation.NgayTao.Year == nam)
                .GroupBy(ct => ct.MaSpNavigation.MaDmNavigation.TenDanhMuc)
                .Select(g => new
                {
                    DanhMuc = g.Key,
                    DoanhThu = g.Sum(ct => ct.SoLuong * ct.DonGia)
                })
                .ToList();

            return Json(data);
        }

        // Tỷ lệ trạng thái đơn hàng (biểu đồ tròn)
        public IActionResult BieuDoTrangThai(int nam)
        {
            var data = _context.DonHangs
                .Where(d => d.NgayTao.Year == nam)
                .GroupBy(d => d.TrangThai)
                .Select(g => new
                {
                    TrangThai = ((TrangThaiDonHang)g.Key).GetDisplayName(),
                    SoLuong = g.Count()
                })
                .ToList();

            return Json(data);
        }


    }
}
