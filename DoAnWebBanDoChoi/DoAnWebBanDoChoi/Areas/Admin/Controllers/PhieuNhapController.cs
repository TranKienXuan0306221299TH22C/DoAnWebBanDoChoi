using DoAnWebBanDoChoi.Filters;
using DoAnWebBanDoChoi.Helpers;
using DoAnWebBanDoChoi.Models;
using DoAnWebBanDoChoi.ViewModels;
using DoAnWebBanDoChoi.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAnWebBanDoChoi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class PhieuNhapController : Controller
    {
        private readonly AppDbContext _context;

        public PhieuNhapController(AppDbContext context)
        {
            _context = context;
        }

        // 1️⃣ Trang danh sách phiếu nhập
        public IActionResult Index()
        {
            var list = _context.PhieuNhaps.Include(p => p.MaNccNavigation).Include(p => p.NguoiNhapNavigation).ToList();
            return View(list);
        }

        // 2️⃣ Trang tạo phiếu nhập (GET)
        public IActionResult Create()
        {
            ViewBag.DanhSachNCC = _context.NhaCungCaps.ToList();
            return View();
        }

        // Trong PhieuNhapController.cs
        [HttpPost]
        public IActionResult Create(TaoPhieuNhapVM viewModel) // 👈 Nhận ViewModel
        {
            if (!ModelState.IsValid)
            {
                // Gửi lại danh sách nhà cung cấp nếu bị lỗi
                ViewBag.DanhSachNCC = _context.NhaCungCaps.ToList();
                return View(viewModel); // 👈 Trả lại View kèm ViewModel
            }

            // 1. Chuyển đổi từ ViewModel sang Entity Model
            var model = new PhieuNhap
            {
                MaNcc = viewModel.MaNcc,
                GhiChu = viewModel.GhiChu,

                // 2. Gán các trường tự động (không cần validation)
                NgayNhap = DateTime.Now,
                TrangThai = 0,
                NguoiNhap = HttpContext.Session.Get<int>("MaNd") // Giả sử MaNd là int
            };

            _context.PhieuNhaps.Add(model);
            _context.SaveChanges();

            
            return RedirectToAction("ChiTiet", new { id = model.MaPn });
        }

        // 3️⃣ Trang chi tiết phiếu nhập
        public IActionResult ChiTiet(int id)
        {
            var pn = _context.PhieuNhaps
                .Include(p => p.ChiTietPhieuNhaps).ThenInclude(c => c.MaSpNavigation)
                .Include(p => p.MaNccNavigation)
                .FirstOrDefault(p => p.MaPn == id);

            if (pn == null) return NotFound();

            return View(pn);
        }

        // 4️⃣ Trang thêm sản phẩm vào chi tiết phiếu nhập (GET)
        public IActionResult ThemChiTiet(int id, string? search)
        {
            ViewBag.MaPn = id;
            ViewBag.CurrentSearch = search;

            // Lấy danh sách sản phẩm đã có trong chi tiết phiếu nhập
            var daCo = _context.ChiTietPhieuNhaps
                .Where(c => c.MaPn == id)
                .Select(c => c.MaSp)
                .ToList();

            // Query sản phẩm chưa thêm vào phiếu
            var query = _context.SanPhams
                .Where(sp => !daCo.Contains(sp.MaSp))
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                string khongDau = SlugHelper.GenerateSlug(search);
                query = query.Where(sp =>
                    sp.TenSanPham.Contains(search) || sp.Slug.Contains(khongDau)
                );
            }

            var danhSach = query.ToList();
            return View(danhSach);
        }


        // 4️⃣ Trang thêm sản phẩm vào chi tiết phiếu nhập (POST)
        [HttpPost]
        public IActionResult ThemChiTiet(ThemChiTietPhieuNhapVM model)
        {
            // 1. Xử lý logic nhập giá nhanh: Nhân giá nhập với 1000
            model.GiaNhap = model.GiaNhap * 1000; // 👈 Dòng code cần thêm

            // 2. Kiểm tra trùng sản phẩm
            bool trung = _context.ChiTietPhieuNhaps
                .Any(c => c.MaPn == model.MaPn && c.MaSp == model.MaSp);

            if (!trung)
            {
                var ct = new ChiTietPhieuNhap
                {
                    MaPn = model.MaPn,
                    MaSp = model.MaSp,
                    SoLuong = model.SoLuong,
                    GiaNhap = model.GiaNhap // Đã là giá trị thực tế sau khi nhân 1000
                };
                _context.ChiTietPhieuNhaps.Add(ct);
                _context.SaveChanges();
                TempData["Success"] = "Thêm sản phẩm thành công.";
            }
            else
            {
                // Đổi thành "Error" để hiển thị cảnh báo
                TempData["Error"] = "⚠️ Sản phẩm đã tồn tại trong phiếu nhập.";
            }

            return RedirectToAction("ChiTiet", new { id = model.MaPn });
        }


        [HttpPost]
        public IActionResult XacNhanPhieuNhap(int id)
        {
            var pn = _context.PhieuNhaps
                .Include(p => p.ChiTietPhieuNhaps)
                .FirstOrDefault(p => p.MaPn == id);

            if (pn == null)
                return NotFound();

            // Nếu đã xác nhận rồi thì không làm lại
            if (pn.TrangThai == 1)
                return RedirectToAction("ChiTiet", new { id });

            // 👉 Kiểm tra nếu không có sản phẩm nào
            if (pn.ChiTietPhieuNhaps == null || !pn.ChiTietPhieuNhaps.Any())
            {
                TempData["Error"] = "⚠️ Phiếu nhập chưa có sản phẩm nào. Vui lòng thêm sản phẩm trước khi xác nhận.";
                return RedirectToAction("ChiTiet", new { id });
            }

            // ✅ Tiến hành cập nhật sản phẩm
            foreach (var ct in pn.ChiTietPhieuNhaps)
            {
                var sp = _context.SanPhams.FirstOrDefault(s => s.MaSp == ct.MaSp);
                if (sp != null)
                {
                    int slCu = sp.SoLuong;
                    decimal giaCu = sp.GiaGoc;
                    int slMoi = ct.SoLuong;
                    decimal giaMoi = ct.GiaNhap;

                    int tongSL = slCu + slMoi;
                    decimal giaTB = (slCu * giaCu + slMoi * giaMoi) / (tongSL);

                    sp.SoLuong = tongSL;
                    sp.GiaGoc = giaTB;
                }
            }

            pn.TrangThai = 1;
            _context.SaveChanges();

            TempData["Success"] = "✔️ Xác nhận phiếu nhập thành công.";
            return RedirectToAction("ChiTiet", new { id });
        }
       
        [HttpPost]
        
        public IActionResult Delete(int id) 
        {
            var pn = _context.PhieuNhaps
                .Include(p => p.ChiTietPhieuNhaps)
                .FirstOrDefault(p => p.MaPn == id);

            if (pn == null)
            {
                TempData["Error"] = "❌ Không tìm thấy phiếu nhập cần xóa.";
                return RedirectToAction("Index");
            }

            // Tái kiểm tra điều kiện trước khi xóa
            if (pn.TrangThai != 0 || (pn.ChiTietPhieuNhaps != null && pn.ChiTietPhieuNhaps.Any()))
            {
                // Đây là kiểm tra logic cốt lõi bạn muốn
                TempData["Error"] = "❌ Chỉ có thể xóa phiếu nhập **Chưa hoàn thành** và **chưa có sản phẩm**.";
                return RedirectToAction("Index");
            }

            _context.PhieuNhaps.Remove(pn);
            _context.SaveChanges();

            TempData["Success"] = $"🗑️ Đã xóa thành công phiếu nhập **Mã PN: {pn.MaPn}**.";
            return RedirectToAction("Index");
        }


    }
}