using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAnWebBanDoChoi.Models;

namespace DoAnWebBanDoChoi.Controllers
{
    public class ChinhSachController : Controller
    {
        private readonly AppDbContext _context;

        public ChinhSachController(AppDbContext context)
        {
            _context = context;
        }

        // Action xem chi tiết
        public async Task<IActionResult> ChiTiet(int id)
        {
            var cs = await _context.ChinhSaches.FirstOrDefaultAsync(m => m.MaCs == id);
            if (cs == null) return NotFound();

            return View(cs);
        }
    }
}