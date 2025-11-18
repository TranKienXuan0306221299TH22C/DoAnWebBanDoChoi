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
            _context = context;
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
                    TieuDe = ph.TieuDe,
                    ThoiGianGui = ph.ThoiGianGui,
                    // TrangThai là bool? -> IsRead là true nếu TrangThai == true, ngược lại là false
                    IsRead = ph.TrangThai ?? false
                })
                .ToList();

            return View(phanHois);
        }
        public async Task<IActionResult> ChiTiet(int id)
        {
            var phanHoi = await _context.PhanHois.FindAsync(id);

            if (phanHoi == null)
            {
                return NotFound();
            }

            // Nếu phản hồi chưa được đánh dấu là đã đọc, tự động đánh dấu
            if (phanHoi.TrangThai == false || phanHoi.TrangThai == null)
            {
                phanHoi.TrangThai = true;
                await _context.SaveChangesAsync();
            }

            // Ánh xạ sang ViewModel để hiển thị
            var model = new PhanHoiVM
            {
                MaPhanHoi = phanHoi.MaPhanHoi,
                HoTen = phanHoi.HoTen,
                Email = phanHoi.Email,
                SoDienThoai = phanHoi.SoDienThoai,
                NoiDung = phanHoi.NoiDung,
                TieuDe = phanHoi.TieuDe,
                ThoiGianGui = phanHoi.ThoiGianGui,
                IsRead = phanHoi.TrangThai ?? false
            };

            return View(model);
        }
        // Action xử lý AJAX để cập nhật trạng thái "Đã xem"
        [HttpPost]
        public async Task<IActionResult> MarkAsReadUnread(int id, bool isRead)
        {
            var phanHoi = await _context.PhanHois.FindAsync(id);

            if (phanHoi == null)
            {
                return Json(new { success = false, message = "Không tìm thấy phản hồi." });
            }

            // Cập nhật trạng thái dựa trên tham số isRead
            phanHoi.TrangThai = isRead;

            try
            {
                await _context.SaveChangesAsync();
                // Trả về JSON để báo hiệu thành công và cập nhật giao diện
                return Json(new { success = true, maPhanHoi = id, isRead = isRead });
            }
            catch (Exception ex)
            {
                // Nên log ex.Message ở đây
                return Json(new { success = false, message = "Lỗi khi cập nhật database." });
            }
        }
    }
}