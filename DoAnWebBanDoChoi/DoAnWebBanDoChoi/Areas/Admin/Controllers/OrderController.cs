using DoAnWebBanDoChoi.Enums;
using DoAnWebBanDoChoi.Filters;
using DoAnWebBanDoChoi.Models;
using DoAnWebBanDoChoi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Extensions;

namespace DoAnWebBanDoChoi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;
        private const int PageSize = 5;
        public OrderController(AppDbContext context)
        {
            _context = context;
        }


        public IActionResult ChoXacNhan(int? page)
        {
            int pageNumber = page ?? 1;
            // Gọi hàm helper mới có tham số pageNumber và PageSize
            var list = GetDonHangTheoTrangThai(TrangThaiDonHang.ChoXacNhan, pageNumber, PageSize);
            ViewBag.TieuDe = "Đơn hàng chờ xác nhận";
            return View("DanhSach", list);
        }

        public IActionResult DaXacNhan(int? page)
        {
            int pageNumber = page ?? 1;
            var list = GetDonHangTheoTrangThai(TrangThaiDonHang.DaXacNhan, pageNumber, PageSize);
            ViewBag.TieuDe = "Đơn hàng đã xác nhận";
            return View("DanhSach", list);
        }

        public IActionResult DangGiao(int? page)
        {
            int pageNumber = page ?? 1;
            var list = GetDonHangTheoTrangThai(TrangThaiDonHang.DangGiao, pageNumber, PageSize);
            ViewBag.TieuDe = "Đơn hàng đang giao";
            return View("DanhSach", list);
        }

        public IActionResult DaGiao(int? page)
        {
            int pageNumber = page ?? 1;
            var list = GetDonHangTheoTrangThai(TrangThaiDonHang.DaGiao, pageNumber, PageSize);
            ViewBag.TieuDe = "Đơn hàng hoàn thành";
            return View("DanhSach", list);
        }

        public IActionResult DaHuy(int? page)
        {
            int pageNumber = page ?? 1;
            var list = GetDonHangTheoTrangThai(TrangThaiDonHang.DaHuy, pageNumber, PageSize);
            ViewBag.TieuDe = "Đơn hàng đã huỷ";
            return View("DanhSach", list);
        }

   
        private IPagedList<DonHangVM> GetDonHangTheoTrangThai(TrangThaiDonHang trangThai, int pageNumber, int pageSize)
        {
            // 1. Tạo Queryable (chưa thực thi)
            var donHangsQuery = _context.DonHangs
                .Where(d => d.TrangThai == (int)trangThai)
                .OrderByDescending(d => d.NgayTao)
                .Select(d => new DonHangVM
                {
                    MaDh = d.MaDh,
                    HoTen = d.HoTen,
                    DienThoai = d.DienThoai,
                    DiaChiDayDu = $"{d.DiaChi}, {d.PhuongXa}, {d.QuanHuyen}, {d.TinhThanh}",
                    NgayTao = d.NgayTao,
                    TongTien = d.TongTien,
                    PhuongThucThanhToan = d.PhuongThucThanhToan,
                    TrangThai = (TrangThaiDonHang)d.TrangThai
                });

            // 2. Thực thi query và áp dụng phân trang (ToPagedList)
            return donHangsQuery.ToPagedList(pageNumber, pageSize);
        }
        public IActionResult ChiTiet(int id)
        {

            var donHang = _context.DonHangs
                .Include(d => d.ChiTietDonHangs)
                .ThenInclude(ct => ct.MaSpNavigation)
                .FirstOrDefault(d => d.MaDh == id);

            if (donHang == null)
            {
                TempData["Error"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction("DonHang");
            }

            return View(donHang);
        }

 
        [HttpPost]
        public IActionResult HuyDon(int id)
        {
            // 1. Nạp đơn hàng và Chi Tiết Đơn Hàng liên quan
            var don = _context.DonHangs
                .Include(d => d.ChiTietDonHangs) // 👈 Quan trọng: Bắt buộc phải có
                .FirstOrDefault(d => d.MaDh == id);

            if (don == null) return NotFound();

            // 2. Chỉ huỷ nếu đang ở 3 trạng thái đầu
            // Đảm bảo không hủy các đơn đã Hoàn thành (DaGiao) hoặc đã Hủy trước đó (DaHuy)
            if (don.TrangThai == (int)TrangThaiDonHang.ChoXacNhan ||
                don.TrangThai == (int)TrangThaiDonHang.DaXacNhan ||
                don.TrangThai == (int)TrangThaiDonHang.DangGiao)
            {
                // 3. HOÀN NHẬP TỒN KHO
                foreach (var chiTiet in don.ChiTietDonHangs)
                {
                    var sp = _context.SanPhams.FirstOrDefault(s => s.MaSp == chiTiet.MaSp);

                    if (sp != null)
                    {
                        // Cộng số lượng đã đặt hàng trở lại vào tồn kho
                        sp.SoLuong += chiTiet.SoLuong;
                    }
                }

                // 4. Cập nhật trạng thái đơn hàng
                don.TrangThai = (int)TrangThaiDonHang.DaHuy;
                don.NgaySua = DateTime.Now;

                // 5. Lưu tất cả thay đổi (cả DonHang và SanPhams)
                _context.SaveChanges();

                TempData["Success"] = $"Đã hủy thành công đơn hàng Mã DH: {id} và hoàn nhập tồn kho.";
            }
            else
            {
                TempData["Error"] = $"Không thể hủy đơn hàng Mã DH: {id} vì đang ở trạng thái hiện tại.";
            }

            // Trở về trang trước (Đây là cách xử lý tốt cho Admin)
            return Redirect(Request.Headers["Referer"].ToString());
        }
        [HttpPost]
        public IActionResult CapNhatTrangThai(int id, TrangThaiDonHang trangThaiMoi)
        {
            var don = _context.DonHangs.FirstOrDefault(d => d.MaDh == id);
            if (don == null) return NotFound();

            don.TrangThai = (int)trangThaiMoi;
            don.NgaySua = DateTime.Now;
            _context.SaveChanges();

            return Redirect(Request.Headers["Referer"].ToString());
        }


    }
}
