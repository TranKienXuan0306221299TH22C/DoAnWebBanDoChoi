using DoAnWebBanDoChoi.Enums;
using DoAnWebBanDoChoi.Helpers;
using DoAnWebBanDoChoi.Models;
using DoAnWebBanDoChoi.Models.VNpay;
using DoAnWebBanDoChoi.Services.VNpay;
using DoAnWebBanDoChoi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAnWebBanDoChoi.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IVnPayService _vnPayService;

        public CheckoutController(AppDbContext context, IVnPayService vnPayService)
        {
            _context = context;
            _vnPayService = vnPayService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var maNd = HttpContext.Session.Get<int>("MaNd");
            if (maNd == 0)
                return RedirectToAction("Login", "Account");

            var gioHang = _context.GioHangs
                .Where(g => g.MaNd == maNd)
                .Include(g => g.MaSpNavigation)
                .Select(g => new GioHangItemVM
                {
                    MaSp = g.MaSp,
                    TenSp = g.MaSpNavigation.TenSanPham,
                    DonGia = g.MaSpNavigation.DonGia,
                    SoLuong = g.SoLuong
                }).ToList();

            var tongTien = gioHang.Sum(x => x.TongTien);
            var phiShip = tongTien > 200_000 ? 0 : 20_000;
            var tongCuoi = tongTien + phiShip;
            ViewBag.TongTien = tongTien;
            var viewModel = new ThanhToanVM
            {
                DanhSachSanPham = gioHang,
                TongTien = tongTien,
                TongTienSauPhiShip = tongCuoi
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult DatHang(ThanhToanVM model)
        {
            var maNd = HttpContext.Session.Get<int>("MaNd");
            if (maNd == 0)
                return RedirectToAction("Login", "Account");

            var gioHang = _context.GioHangs
                .Where(g => g.MaNd == maNd)
                .Include(g => g.MaSpNavigation)
                .ToList();

            if (!gioHang.Any())
                return RedirectToAction("ChiTietGioHang", "Cart");

            decimal tongTien = gioHang.Sum(sp => sp.SoLuong * sp.MaSpNavigation.DonGia);
            decimal phiShip = tongTien > 200_000 ? 0 : 20_000;
            decimal tongCuoi = tongTien + phiShip;

            string phuongThuc = model.PhuongThucThanhToan?.ToLower() ?? "";

            if (phuongThuc == "vnpay")
            {
                HttpContext.Session.Set("ThongTinDatHang", model);
                HttpContext.Session.Set("TongTienThanhToan", tongCuoi);

                long amount = Convert.ToInt64(tongCuoi * 100);
                if (amount < 5000 || amount >= 1_000_000_000)
                {
                    TempData["Error"] = "Số tiền không hợp lệ. Chỉ từ 5.000đ đến dưới 1 triệu.";
                    return RedirectToAction("ChiTietGioHang", "Cart");
                }

                var payment = new PaymentInformationModel
                {
                    Amount = amount,
                    Name = model.HoTen,
                    OrderDescription = "Đơn hàng thanh toán VNPay",
                    OrderType = "billpayment"
                };

                var url = _vnPayService.CreatePaymentUrl(payment, HttpContext);
                return Redirect(url);
            }
            else if (phuongThuc != "cod")
            {
                TempData["Error"] = "Phương thức thanh toán không hợp lệ.";
                return RedirectToAction("ChiTietGioHang", "Cart");
            }

            // === Đặt hàng COD ===
            var donHang = new DonHang
            {
                MaNd = maNd,
                HoTen = model.HoTen,
                DienThoai = model.DienThoai,
                DiaChi = model.DiaChi,
                GhiChu = model.GhiChu,
                TongTien = tongCuoi,
                PhiVanChuyen = phiShip,
                PhuongThucThanhToan = "COD",
                TrangThai = (int)TrangThaiDonHang.ChoXacNhan,
                NgayTao = DateTime.Now
            };

            _context.DonHangs.Add(donHang);
            _context.SaveChanges();

            foreach (var item in gioHang)
            {
                _context.ChiTietDonHangs.Add(new ChiTietDonHang
                {
                    MaDh = donHang.MaDh,
                    MaSp = item.MaSp,
                    SoLuong = item.SoLuong,
                    DonGia = item.MaSpNavigation.DonGia
                });
            }

            _context.GioHangs.RemoveRange(gioHang);
            _context.SaveChanges();

            TempData["Success"] = "Đặt hàng thành công!";
            return RedirectToAction("ChiTietGioHang", "Cart");
        }

        [HttpGet]
        public IActionResult PaymentCallbackVnpay()
        {
            var maNd = HttpContext.Session.Get<int>("MaNd");
            if (maNd == 0)
                return RedirectToAction("Login", "Account");

            var response = _vnPayService.PaymentExecute(Request.Query);

            if (response.Success && response.VnPayResponseCode == "00")
            {
                var model = HttpContext.Session.Get<ThanhToanVM>("ThongTinDatHang");
                var tongCuoi = HttpContext.Session.Get<decimal>("TongTienThanhToan");

                var gioHang = _context.GioHangs
                    .Where(g => g.MaNd == maNd)
                    .Include(g => g.MaSpNavigation)
                    .ToList();

                decimal tongTien = gioHang.Sum(sp => sp.SoLuong * sp.MaSpNavigation.DonGia);
                decimal phiShip = tongCuoi - tongTien;

                var donHang = new DonHang
                {
                    MaNd = maNd,
                    HoTen = model.HoTen,
                    DienThoai = model.DienThoai,
                    DiaChi = model.DiaChi,
                    GhiChu = model.GhiChu,
                    TongTien = tongCuoi,
                    PhiVanChuyen = phiShip,
                    PhuongThucThanhToan = "VNPay",
                    TrangThai = (int)TrangThaiDonHang.ChoXacNhan,
                    NgayTao = DateTime.Now
                };

                _context.DonHangs.Add(donHang);
                _context.SaveChanges();

                foreach (var item in gioHang)
                {
                    _context.ChiTietDonHangs.Add(new ChiTietDonHang
                    {
                        MaDh = donHang.MaDh,
                        MaSp = item.MaSp,
                        SoLuong = item.SoLuong,
                        DonGia = item.MaSpNavigation.DonGia
                    });
                }

                _context.GioHangs.RemoveRange(gioHang);
                _context.SaveChanges();

                TempData["Success"] = "Thanh toán VNPay thành công!";
            }
            else
            {
                TempData["Error"] = $"Thanh toán thất bại (Mã: {response.VnPayResponseCode})";
            }

            return RedirectToAction("ChiTietGioHang", "Cart");
        }

    }
}