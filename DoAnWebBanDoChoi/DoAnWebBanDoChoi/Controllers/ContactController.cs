using DoAnWebBanDoChoi.Helpers;
using DoAnWebBanDoChoi.Models;
using DoAnWebBanDoChoi.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DoAnWebBanDoChoi.Controllers
{
    public class ContactController : Controller
    {
        private readonly AppDbContext _context;

        public ContactController(AppDbContext context)
        {
            _context = context;
        }
       
        public IActionResult LienHe()
        {
            // Trả về một ContactVM mới rỗng để chuẩn bị cho form
            return View(new ContactVM());
        }
        [HttpPost]
        public IActionResult LienHe(ContactVM model)
        {
            if (ModelState.IsValid)
            {
                var phanHoi = new PhanHoi
                {
                    HoTen = model.HoTen,
                    Email = model.Email,
                    SoDienThoai = model.SoDienThoai,
                    NoiDung = model.NoiDung,
                    TieuDe = model.TieuDe,
                    ThoiGianGui = DateTime.Now,
                    TrangThai = false // Luôn là 'Chưa đọc' khi mới gửi
                };
                try
                {
                    _context.PhanHois.Add(phanHoi);
                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Cảm ơn bạn đã gửi ý kiến! Shop đã nhận được nội dung liên hệ của bạn.";
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Đã xảy ra lỗi khi lưu dữ liệu. Vui lòng thử lại sau.");
                    return View(model);
                }
                // Chuyển hướng về GET LienHe để xóa dữ liệu form và hiển thị thông báo
                return RedirectToAction("LienHe");
            }

            // Nếu model không hợp lệ, trả về View với model cũ để giữ lại dữ liệu và hiển thị lỗi
            return View(model);
        }
    }
}
