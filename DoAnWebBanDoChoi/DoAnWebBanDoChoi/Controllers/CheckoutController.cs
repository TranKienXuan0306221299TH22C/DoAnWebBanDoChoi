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
            var viewModel = new ThanhToanVM
            {
                DanhSachSanPham = gioHang,
                TongTien = tongTien,
                TongTienSauPhiShip = tongTien
            };

            ViewBag.TongTien = tongTien;
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult DatHang(ThanhToanVM model)
        {
            var maNd = HttpContext.Session.Get<int>("MaNd");
            if (maNd == 0)
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                var giohang = _context.GioHangs
                    .Where(g => g.MaNd == maNd)
                    .Include(g => g.MaSpNavigation)
                    .ToList();

                model.DanhSachSanPham = giohang.Select(g => new GioHangItemVM
                {
                    MaSp = g.MaSp,
                    TenSp = g.MaSpNavigation.TenSanPham,
                    DonGia = g.MaSpNavigation.DonGia,
                    SoLuong = g.SoLuong
                }).ToList();

                model.TongTien = model.DanhSachSanPham.Sum(sp => sp.TongTien);
                model.TongTienSauPhiShip = model.TongTien;

                return View("Index", model);
            }
            var gioHang = _context.GioHangs
                .Where(g => g.MaNd == maNd)
                .Include(g => g.MaSpNavigation)
                .ToList();

            if (!gioHang.Any())
                return RedirectToAction("ChiTietGioHang", "Cart");

            decimal tongTien = gioHang.Sum(sp => sp.SoLuong * sp.MaSpNavigation.DonGia);
            decimal phiShip = _context.PhiVanChuyens
                .FirstOrDefault(p => p.TenTinh == model.TinhThanh && p.TenHuyen == model.QuanHuyen)?.PhiShip ?? 20000;

            if (tongTien > 300_000)
                phiShip = 0;

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

            // ✅ Đặt hàng COD
            if (!XuLyDonHang(model, maNd, "COD", tongTien, phiShip, out string error))
            {
                TempData["Error"] = error;
                return RedirectToAction("ChiTietGioHang", "Cart");
            }

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

                if (!XuLyDonHang(model, maNd, "VNPay", tongTien, phiShip, out string error))
                {
                    TempData["Error"] = error;
                    return RedirectToAction("ChiTietGioHang", "Cart");
                }

                TempData["Success"] = "Thanh toán VNPay thành công!";
            }
            else
            {
                TempData["Error"] = $"Thanh toán thất bại (Mã: {response.VnPayResponseCode})";
            }

            return RedirectToAction("ChiTietGioHang", "Cart");
        }

        [HttpPost]
        public IActionResult LayPhiShip(string tinh, string huyen, decimal tongTien)
        {
            decimal phiShip = _context.PhiVanChuyens
                .FirstOrDefault(p => p.TenTinh == tinh && p.TenHuyen == huyen)?.PhiShip ?? 20000;

            if (tongTien > 300000)
                phiShip = 0;

            return Json(new { phiShip });
        }

        // ✅ HÀM CHUNG XỬ LÝ ĐƠN HÀNG
        private bool XuLyDonHang(ThanhToanVM model, int maNd, string phuongThuc, decimal tongTien, decimal phiShip, out string error)
        {
            error = "";

            var gioHang = _context.GioHangs
                .Where(g => g.MaNd == maNd)
                .Include(g => g.MaSpNavigation)
                .ToList();

            // ✅ Kiểm tra còn hàng không
            foreach (var item in gioHang)
            {
                var sp = item.MaSpNavigation;
                if (item.SoLuong > sp.SoLuong)
                {
                    error = $"Sản phẩm '{sp.TenSanPham}' chỉ còn {sp.SoLuong} cái trong kho.";
                    return false;
                }
            }

            var donHang = new DonHang
            {
                MaNd = maNd,
                HoTen = model.HoTen,
                DienThoai = model.DienThoai,
                DiaChi = model.DiaChi,
                GhiChu = model.GhiChu,
                TongTien = tongTien + phiShip,
                PhiVanChuyen = phiShip,
                PhuongThucThanhToan = phuongThuc,
                TinhThanh = model.TinhThanh,
                QuanHuyen = model.QuanHuyen,
                PhuongXa = model.PhuongXa,
                TrangThai = (int)TrangThaiDonHang.ChoXacNhan,
                NgayTao = DateTime.Now
            };

            _context.DonHangs.Add(donHang);
            _context.SaveChanges();

            foreach (var item in gioHang)
            {
                var sp = item.MaSpNavigation;

                _context.ChiTietDonHangs.Add(new ChiTietDonHang
                {
                    MaDh = donHang.MaDh,
                    MaSp = sp.MaSp,
                    SoLuong = item.SoLuong,
                    DonGia = sp.DonGia,
                    GiaGoc = sp.GiaGoc
                });

                sp.SoLuong -= item.SoLuong;
            }

            _context.GioHangs.RemoveRange(gioHang);
            _context.SaveChanges();

            return true;
        }
    }
}
