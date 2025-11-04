using DoAnWebBanDoChoi.Filters;
using DoAnWebBanDoChoi.Models;
using DoAnWebBanDoChoi.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;

namespace DoAnWebBanDoChoi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class ContactController : Controller
    {
        private readonly AppDbContext _context;

        public ContactController(AppDbContext context)
        {
            _context=context;
        }
        public IActionResult LienHe()
        {
            // Lấy tất cả phản hồi và ánh xạ sang ViewModel
            var phanHois = _context.PhanHois
                .OrderByDescending(ph => ph.ThoiGianGui)
                .Select(ph => new PhanHoiVM
                {
                    MaPhanHoi = ph.MaPhanHoi,
                    HoTen = ph.HoTen,
                    Email = ph.Email,
                    SoDienThoai = ph.SoDienThoai,
                    NoiDung = ph.NoiDung,
                    ThoiGianGui = ph.ThoiGianGui,
                    // TrangThai là bool? -> IsRead là true nếu TrangThai == true, ngược lại là false
                    IsRead = ph.TrangThai ?? false
                })
                .ToList();

            return View(phanHois);
        }

        // Action xử lý AJAX để cập nhật trạng thái "Đã xem"
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var phanHoi = await _context.PhanHois.FindAsync(id);

            if (phanHoi == null)
            {
                return NotFound();
            }

            // Cập nhật trạng thái: true = đã xem
            phanHoi.TrangThai = true;

            try
            {
                await _context.SaveChangesAsync();
                // Trả về JSON để báo hiệu thành công và cập nhật giao diện
                return Json(new { success = true, maPhanHoi = id });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi (ví dụ: log lỗi)
                return Json(new { success = false, message = "Lỗi khi cập nhật database." });
            }
        }
    }
}
