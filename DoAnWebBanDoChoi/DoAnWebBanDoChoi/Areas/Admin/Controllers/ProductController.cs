using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAnWebBanDoChoi.Models;
using DoAnWebBanDoChoi.ViewModels;
using DoAnWebBanDoChoi.Helpers;

namespace DoAnWebBanDoChoi.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        //public IActionResult SanPham()
        //{
        //    var sanPhams = _context.SanPhams
        //        .Include(sp => sp.MaDmNavigation)
        //        .Include(sp => sp.MaThNavigation)
        //        .Include(sp => sp.MaNccNavigation)
        //        .ToList();
        //    return View(sanPhams);
        //}
        public IActionResult SanPham(string? search, int? maDm)
        {
            var query = _context.SanPhams
                .Include(sp => sp.MaDmNavigation)
                .Include(sp => sp.MaThNavigation)
                .Include(sp => sp.MaNccNavigation)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
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

            ViewBag.DanhMucs = _context.DanhMucs.ToList();
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentDanhMuc = maDm;

            var sanPhamList = query.ToList();
            return View(sanPhamList);
        }


        public IActionResult Create()
        {
            ViewBag.DanhMucs = _context.DanhMucs.ToList();
            ViewBag.ThuongHieus = _context.ThuongHieus.ToList();
            ViewBag.NhaCungCaps = _context.NhaCungCaps.ToList();
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
                    HinhAnh = fileName,
                    MaDm = model.MaDm.Value,
                    MaTh = model.MaTh.Value,
                    MaNcc = model.MaNcc.Value,
                    Slug = SlugHelper.GenerateSlug(model.TenSanPham),
                    NgayTao = DateTime.Now,
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
            sp.MaDm = model.MaDm.Value;
            sp.MaTh = model.MaTh.Value;
            sp.MaNcc = model.MaNcc.Value;
            sp.DonGia = model.DonGia; 
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
            var sp = _context.SanPhams.Find(id);
            if (sp == null)
                return Json(new { success = false, message = "Sản phẩm không tồn tại!" });

            if (sp.TrangThai == 0) // Đang ngừng bán → muốn chuyển sang hoạt động
            {
                if (sp.GiaGoc <= 0 || sp.DonGia <= 0 || sp.SoLuong <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Không thể mở bán! Vui lòng kiểm tra giá nhập, giá bán và số lượng."
                    });
                }

                sp.TrangThai = 1; // Bật trạng thái
            }
            else
            {
                sp.TrangThai = 0; // Ngưng bán thì lúc nào cũng cho phép
            }

            sp.NgaySua = DateTime.Now;
            _context.SaveChanges();

            return Json(new
            {
                success = true,
                newStatus = sp.TrangThai
            });
        }













    }
}