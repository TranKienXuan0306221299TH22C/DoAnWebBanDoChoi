using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAnWebBanDoChoi.Models; // Thay bằng namespace chứa DbContext của bạn

namespace DoAnWebBanDoChoi.ViewComponents
{
    public class CauHinhViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public CauHinhViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Lấy tất cả chính sách (hoặc lấy vài cái nếu danh sách quá dài)
            var cauHinhs = await _context.CauHinhs.ToListAsync();
            return View(cauHinhs);
        }
    }
}