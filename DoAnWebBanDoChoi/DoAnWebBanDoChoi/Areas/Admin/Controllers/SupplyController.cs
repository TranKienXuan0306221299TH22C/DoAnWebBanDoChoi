using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAnWebBanDoChoi.Models;
using DoAnWebBanDoChoi.Filters;

namespace DoAnWebBanDoChoi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class SupplyController : Controller
    {
        private readonly AppDbContext _context;

        public SupplyController(AppDbContext context)
        {
            _context = context;
        }

        
        public IActionResult NhaCungCap()
        {
            var ncc = _context.NhaCungCaps.ToList();
            return View(ncc);
        }

       
        public IActionResult Create()
        {
            return View();
        }

       
        [HttpPost]
       
        public IActionResult Create(NhaCungCap nhaCungCap)
        {
            if (ModelState.IsValid)
            {
                _context.NhaCungCaps.Add(nhaCungCap);
                _context.SaveChanges();
                TempData["Success"] = "Thêm nhà cung cấp thành công!";
                return RedirectToAction("NhaCungCap");
            }
            return View(nhaCungCap);
        }

       
        public IActionResult Edit(int id)
        {
            var ncc = _context.NhaCungCaps.Find(id);
            if (ncc == null)
            {
                return NotFound();
            }
            return View(ncc);
        }


        [HttpPost]
        public IActionResult Edit(int id, NhaCungCap nhaCungCap)
        {
            if (id != nhaCungCap.MaNcc) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(nhaCungCap);
                _context.SaveChanges();

                TempData["Success"] = "Cập nhật nhà cung cấp thành công!";
                return RedirectToAction("NhaCungCap");
            }

            return View(nhaCungCap);
        }



        public IActionResult DanhSachXoa()
        {
            var ds = _context.NhaCungCaps
                .Include(x => x.SanPhams)
                .Include(x => x.PhieuNhaps)
                .Where(x => !x.SanPhams.Any() && !x.PhieuNhaps.Any())
                .ToList();

            return View(ds);
        }

        
        [HttpPost]
       
        public IActionResult DeleteConfirmed(int id)
        {
            var ncc = _context.NhaCungCaps.Find(id);
            if (ncc != null)
            {
                _context.NhaCungCaps.Remove(ncc);
                _context.SaveChanges();
                TempData["Success"] = "Xóa nhà cung cấp thành công!";
            }

            return RedirectToAction("DanhSachXoa");
        }
    }
}
