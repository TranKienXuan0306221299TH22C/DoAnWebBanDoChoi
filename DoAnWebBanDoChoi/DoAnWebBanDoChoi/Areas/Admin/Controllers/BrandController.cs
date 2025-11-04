using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAnWebBanDoChoi.Models;
using DoAnWebBanDoChoi.Filters;


namespace DoAnWebBanDoChoi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class BrandController : Controller
    {
        private readonly AppDbContext _context;

        public BrandController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult ThuongHieu()
        {
            var brand = _context.ThuongHieus.ToList();

            return View(brand);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ThuongHieu thuongHieu)
        {
            if (ModelState.IsValid)
            {
                _context.ThuongHieus.Add(thuongHieu);
                _context.SaveChanges();
                TempData["Success"] = "Thêm thương hiệu thành công!";
                return RedirectToAction("ThuongHieu");
            }

            return View();
        }
        public IActionResult Edit(int id)
        {
            var thuonghieu = _context.ThuongHieus.Find(id);
            if (thuonghieu == null)
            {
                return NotFound();
            }
            return View(thuonghieu);
        }
        [HttpPost]
        public IActionResult Edit(int id, ThuongHieu thuongHieu)
        {
            if (id != thuongHieu.MaTh)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(thuongHieu);
                _context.SaveChanges();
                TempData["Success"] = "Cập nhật thương hiệu thành công!";
                return RedirectToAction("ThuongHieu");
            }

            return View(thuongHieu);
        }

        public IActionResult DanhSachXoa()
        {
            var danhMucs = _context.ThuongHieus
                .Include(th => th.SanPhams)
                .Where(th => !th.SanPhams.Any())
                .ToList();

            return View(danhMucs);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            var th = _context.ThuongHieus.Find(id);
            if (th != null)
            {
                _context.ThuongHieus.Remove(th);
                _context.SaveChanges();
                TempData["Success"] = "Xóa thương hiệu thành công!";
            }

            return RedirectToAction("DanhSachXoa");
        }

    }
}
