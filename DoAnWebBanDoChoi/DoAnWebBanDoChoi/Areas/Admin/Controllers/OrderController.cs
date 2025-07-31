using Microsoft.AspNetCore.Mvc;
using DoAnWebBanDoChoi.Models;
using DoAnWebBanDoChoi.Enums;
using DoAnWebBanDoChoi.Filters;
using DoAnWebBanDoChoi.ViewModels;

namespace DoAnWebBanDoChoi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult ChoXacNhan()
        {
            var list = GetDonHangTheoTrangThai(TrangThaiDonHang.ChoXacNhan);
            ViewBag.TieuDe = "Đơn hàng chờ xác nhận";
            return View("DanhSach", list);
        }

        public IActionResult DaXacNhan()
        {
            var list = GetDonHangTheoTrangThai(TrangThaiDonHang.DaXacNhan);
            ViewBag.TieuDe = "Đơn hàng đã xác nhận";
            return View("DanhSach", list);
        }

        public IActionResult DangGiao()
        {
            var list = GetDonHangTheoTrangThai(TrangThaiDonHang.DangGiao);
            ViewBag.TieuDe = "Đơn hàng đang giao";
            return View("DanhSach", list);
        }

        public IActionResult DaGiao()
        {
            var list = GetDonHangTheoTrangThai(TrangThaiDonHang.DaGiao);
            ViewBag.TieuDe = "Đơn hàng hoàn thành";
            return View("DanhSach", list);
        }

        public IActionResult DaHuy()
        {
            var list = GetDonHangTheoTrangThai(TrangThaiDonHang.DaHuy);
            ViewBag.TieuDe = "Đơn hàng đã huỷ";
            return View("DanhSach", list);
        }

        [HttpPost]
        public IActionResult HuyDon(int id)
        {
            var don = _context.DonHangs.FirstOrDefault(d => d.MaDh == id);
            if (don == null) return NotFound();

            // Chỉ huỷ nếu đang ở 3 trạng thái đầu
            if (don.TrangThai == (int)TrangThaiDonHang.ChoXacNhan ||
                don.TrangThai == (int)TrangThaiDonHang.DaXacNhan ||
                don.TrangThai == (int)TrangThaiDonHang.DangGiao)
            {
                don.TrangThai = (int)TrangThaiDonHang.DaHuy;
                don.NgaySua = DateTime.Now;
                _context.SaveChanges();
            }

            // Trở về trang trước
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

        private List<DonHangVM> GetDonHangTheoTrangThai(TrangThaiDonHang trangThai)
        {
            return _context.DonHangs
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
                })
                .ToList();
        }
    }
}
