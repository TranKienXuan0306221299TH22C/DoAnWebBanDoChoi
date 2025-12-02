using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAnWebBanDoChoi.Models; // Thay bằng namespace chứa DbContext của bạn

namespace DoAnWebBanDoChoi.ViewComponents
{
    public class ChinhSachFooterViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public ChinhSachFooterViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Lấy tất cả chính sách (hoặc lấy vài cái nếu danh sách quá dài)
            var chinhSachs = await _context.ChinhSaches.ToListAsync();
            return View(chinhSachs);
        }
    }
}