using DoAnWebBanDoChoi.Filters;
using DoAnWebBanDoChoi.Helpers;
using DoAnWebBanDoChoi.Models;
using DoAnWebBanDoChoi.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace DoAnWebBanDoChoi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

   
        public IActionResult SanPham(string? search, int? maDm, int? page)
        {
            int pageNumber = page ?? 1; 
            int pageSize = 5;
            var query = _context.SanPhams
                .Include(sp => sp.MaDmNavigation)
                .Include(sp => sp.MaThNavigation)
                .Include(sp => sp.MaNccNavigation)
                .AsQueryable();
                

            if (!string.IsNullOrWhiteSpace(search))
            {
                string khongDau = SlugHelper.GenerateSlug(search);
                query = query.Where(sp =>
                    sp.TenSanPham.Contains(search) || sp.Slug.Contains(khongDau)
                );
            }

            if (maDm.HasValue)
            {
                query = query.Where(sp => sp.MaDm == maDm.Value);
            }
            var sanPhamPagedList = query.ToPagedList(pageNumber, pageSize);

            ViewBag.DanhMucs = _context.DanhMucs.ToList();
            ViewBag.CurrentSearch = search ?? "";
            ViewBag.CurrentDanhMuc = maDm;
            ViewBag.CurrentPage = pageNumber;
            

            return View(sanPhamPagedList);
        }


        private List<string> GetAgeSuggestions()
        {
            return new List<string>
    {
        "Bé từ 3 tuổi trở lên",
        "Bé từ 1 tuổi trở lên",
        "Trẻ từ 3 đến 5 tuổi"
    };
        }
        public IActionResult Create()
        {
            ViewBag.DanhMucs = _context.DanhMucs.ToList();
            ViewBag.ThuongHieus = _context.ThuongHieus.ToList();
            ViewBag.NhaCungCaps = _context.NhaCungCaps.ToList();
            ViewBag.AgeSuggestions = GetAgeSuggestions();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductVM model)
        {
            if (ModelState.IsValid)
            {
                string? fileName = null;
                if (model.HinhAnhFile != null)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/product/dochoi");
                    fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.HinhAnhFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.HinhAnhFile.CopyToAsync(stream);
                    }
                }

                var sp = new SanPham
                {
                    TenSanPham = model.TenSanPham,
                    MoTa = model.MoTa,
                    ChatLieu = model.ChatLieu,
                    KichThuoc = model.KichThuoc,
                    DoTuoiPhuHop = model.DoTuoiPhuHop,
                    TrongLuong = model.TrongLuong,
                    HinhAnh = fileName,
                    MaDm = model.MaDm.Value,
                    MaTh = model.MaTh.Value,
                    MaNcc = model.MaNcc.Value,
                    Slug = SlugHelper.GenerateSlug(model.TenSanPham),
                    NgayTao = DateTime.Now,
                    GiaGoc = 0,
                    DonGia = 0,
                    SoLuong = 0,
                    TrangThai = 0
                };

                _context.SanPhams.Add(sp);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm sản phẩm thành công!";
                return RedirectToAction("SanPham");
            }

            ViewBag.DanhMucs = _context.DanhMucs.ToList();
            ViewBag.ThuongHieus = _context.ThuongHieus.ToList();
            ViewBag.NhaCungCaps = _context.NhaCungCaps.ToList();
            ViewBag.AgeSuggestions = GetAgeSuggestions();
            return View(model);
        }

        public IActionResult Edit(int id)
        {
            var sp = _context.SanPhams.Find(id);
            if (sp == null) return NotFound();

            var vm = new ProductVM
            {
                TenSanPham = sp.TenSanPham,
                MoTa = sp.MoTa,
                ChatLieu = sp.ChatLieu,
                KichThuoc = sp.KichThuoc,
                DoTuoiPhuHop = sp.DoTuoiPhuHop,
                TrongLuong = sp.TrongLuong,
                MaDm = sp.MaDm,
                MaTh = sp.MaTh,
                MaNcc = sp.MaNcc,
                DonGia = sp.DonGia
            };

            ViewBag.DanhMucs = _context.DanhMucs.ToList();
            ViewBag.ThuongHieus = _context.ThuongHieus.ToList();
            ViewBag.NhaCungCaps = _context.NhaCungCaps.ToList();
            ViewBag.HinhAnhCu = sp.HinhAnh;

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ProductVM model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.DanhMucs = _context.DanhMucs.ToList();
                ViewBag.ThuongHieus = _context.ThuongHieus.ToList();
                ViewBag.NhaCungCaps = _context.NhaCungCaps.ToList();
                return View(model);
            }

            var sp = _context.SanPhams.Find(id);
            if (sp == null) return NotFound();

            // Cập nhật thông tin
            sp.TenSanPham = model.TenSanPham;
            sp.MoTa = model.MoTa;
            sp.ChatLieu = model.ChatLieu;
            sp.KichThuoc = model.KichThuoc;
            sp.DoTuoiPhuHop = model.DoTuoiPhuHop;
            sp.TrongLuong = model.TrongLuong;
            sp.MaDm = model.MaDm.Value;
            sp.MaTh = model.MaTh.Value;
            sp.MaNcc = model.MaNcc.Value;
            sp.DonGia = model.DonGia ?? 0;
            sp.NgaySua = DateTime.Now;

            // Cập nhật ảnh nếu có
            if (model.HinhAnhFile != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/product/dochoi");
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.HinhAnhFile.FileName);
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.HinhAnhFile.CopyToAsync(stream);
                }

                sp.HinhAnh = fileName;
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Cập nhật sản phẩm thành công!";
            return RedirectToAction("SanPham");
        }

        [HttpPost]
        public JsonResult ToggleStatusAjax(int id)
        {
            // 1. Tìm sản phẩm
            var sp = _context.SanPhams.Find(id);
            if (sp == null)
                return Json(new { success = false, message = "Sản phẩm không tồn tại!" });

            // 2. Xử lý logic chuyển từ Ngừng Bán (0) sang Hoạt Động (1)
            if (sp.TrangThai == 0)
            {
                // 2a. Kiểm tra điều kiện cần thiết để mở bán (SL, Giá Gốc, Giá Bán > 0)
                if (sp.GiaGoc <= 0 || sp.DonGia <= 0 || sp.SoLuong <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "❌ Không thể mở bán! Vui lòng kiểm tra: Giá nhập, Giá bán, và Số lượng tồn kho (phải > 0)."
                    });
                }

                // 2b. (Logic BỔ SUNG): Kiểm tra Giá bán phải lớn hơn Giá gốc (tránh bán lỗ)
                if (sp.DonGia < sp.GiaGoc)
                {
                    return Json(new
                    {
                        success = false,
                        message = "⚠️ Cảnh báo! Giá bán đang thấp hơn Giá gốc. Vui lòng điều chỉnh giá bán cao hơn."
                    });
                }

                // Nếu tất cả điều kiện thỏa mãn, cho phép mở bán
                sp.TrangThai = 1;
            }
            // 3. Xử lý logic chuyển từ Hoạt Động (1) sang Ngừng Bán (0)
            else
            {
                // Cho phép ngừng bán bất cứ lúc nào
                sp.TrangThai = 0;
            }

            // 4. Lưu thay đổi và trả về kết quả
            sp.NgaySua = DateTime.Now;
            _context.SaveChanges();

            // Chuẩn bị thông báo trả về
            string successMessage = (sp.TrangThai == 1) ? "✔️ Đã mở bán sản phẩm thành công." : "⏸️ Đã ngừng bán sản phẩm.";

            return Json(new
            {
                success = true,
                newStatus = sp.TrangThai,
                message = successMessage // Trả về thông báo thành công mới
            });
        }
    }
}