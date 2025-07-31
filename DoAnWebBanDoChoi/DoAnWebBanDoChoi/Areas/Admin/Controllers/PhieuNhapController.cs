using DoAnWebBanDoChoi.Helpers;
using DoAnWebBanDoChoi.Models;
using DoAnWebBanDoChoi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAnWebBanDoChoi.Areas.Admin.Controllers
{
    [Area("Admin")]
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
            var list = _context.PhieuNhaps.Include(p => p.MaNccNavigation).ToList();
            return View(list);
        }

        // 2️⃣ Trang tạo phiếu nhập (GET)
        public IActionResult Create()
        {
            ViewBag.DanhSachNCC = _context.NhaCungCaps.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(PhieuNhap model)
        {
            if (!ModelState.IsValid)
            {
                // Gửi lại danh sách nhà cung cấp nếu bị lỗi
                ViewBag.DanhSachNCC = _context.NhaCungCaps.ToList();
                return View(model); // Trả lại View kèm lỗi
            }

            model.NgayNhap = DateTime.Now;
            model.TrangThai = 0;
            model.NguoiNhap = HttpContext.Session.Get<int>("MaNd");

            _context.PhieuNhaps.Add(model);
            _context.SaveChanges();

            TempData["Success"] = "Tạo phiếu nhập thành công. Bạn có thể thêm sản phẩm.";
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
            // Kiểm tra trùng sản phẩm
            bool trung = _context.ChiTietPhieuNhaps
                .Any(c => c.MaPn == model.MaPn && c.MaSp == model.MaSp);

            if (!trung)
            {
                var ct = new ChiTietPhieuNhap
                {
                    MaPn = model.MaPn,
                    MaSp = model.MaSp,
                    SoLuong = model.SoLuong,
                    GiaNhap = model.GiaNhap
                };
                _context.ChiTietPhieuNhaps.Add(ct);
                _context.SaveChanges();
                TempData["Success"] = "Thêm sản phẩm thành công.";
            }
            else
            {
                TempData["Success"] = "⚠️ Sản phẩm đã tồn tại trong phiếu nhập.";
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


    }
}
