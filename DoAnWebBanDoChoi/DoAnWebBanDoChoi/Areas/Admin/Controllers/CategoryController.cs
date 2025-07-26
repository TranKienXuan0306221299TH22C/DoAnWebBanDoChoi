using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAnWebBanDoChoi.Models;
using DoAnWebBanDoChoi.Filters;

namespace DoAnWebBanDoChoi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult DanhMuc()
        {
            var danhmuc = _context.DanhMucs.ToList();
            return View(danhmuc);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(DanhMuc danhMuc)
        {
            if(ModelState.IsValid)
            {
                _context.DanhMucs.Add(danhMuc);
                _context.SaveChanges();
                TempData["Success"] = "Thêm danh mục thành công!";
                return RedirectToAction("DanhMuc");
            }

            return View();
        }
        public IActionResult Edit(int id)
        {
            var danhMuc = _context.DanhMucs.Find(id);
            if (danhMuc == null)
            {
                return NotFound();
            }
            return View(danhMuc);
        }
        [HttpPost]
        public IActionResult Edit(int id, DanhMuc danhMuc)
        {
            if (id != danhMuc.MaDm)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(danhMuc);
                _context.SaveChanges();
                TempData["Success"] = "Cập nhật danh mục thành công!";
                return RedirectToAction("DanhMuc");
            }

            return View(danhMuc);
        }
       
        public IActionResult DanhSachXoa()
        {
            var danhMucs = _context.DanhMucs
                .Include(dm => dm.SanPhams)
                .Where(dm => !dm.SanPhams.Any()) 
                .ToList();

            return View(danhMucs);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            var dm = _context.DanhMucs.Find(id);
            if (dm != null)
            {
                _context.DanhMucs.Remove(dm);
                _context.SaveChanges();
                TempData["Success"] = "Xóa danh mục thành công!";
            }

            return RedirectToAction("DanhSachXoa");
        }








    }
}
