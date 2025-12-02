using DoAnWebBanDoChoi.Filters;
using DoAnWebBanDoChoi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAnWebBanDoChoi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class ChinhSachController : Controller
    {
        private readonly AppDbContext _context;

        public ChinhSachController(AppDbContext context)
        {
            _context = context;
        }

        // 1. Hiển thị danh sách
        public async Task<IActionResult> Index()
        {
            var data = await _context.ChinhSaches.ToListAsync();
            return View(data);
        }

        // 2. GET: Hiển thị form Thêm mới
        public IActionResult Create()
        {
            return View();
        }

        // 2. POST: Xử lý thêm mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ChinhSach model)
        {
            if (ModelState.IsValid)
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // 3. GET: Hiển thị form Sửa
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var chinhSach = await _context.ChinhSaches.FindAsync(id);
            if (chinhSach == null) return NotFound();

            return View(chinhSach);
        }

        // 3. POST: Xử lý cập nhật
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ChinhSach model)
        {
            if (id != model.MaCs) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ChinhSaches.Any(e => e.MaCs == model.MaCs)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // 4. GET: Xác nhận xóa (Hoặc xóa trực tiếp tùy logic của bạn)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var chinhSach = await _context.ChinhSaches.FindAsync(id);
            if (chinhSach != null)
            {
                _context.ChinhSaches.Remove(chinhSach);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}