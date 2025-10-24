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

        // POST: /Home/LienHe (Xử lý gửi form)
        [HttpPost]
        public IActionResult LienHe(ContactVM model)
        {
            if (ModelState.IsValid)
            {
                // 1. Lấy MaNd từ Session
                // Nếu người dùng chưa đăng nhập, MaNd sẽ là 0
                

                // 2. Map dữ liệu từ ViewModel sang Entity Model PhanHoi
                var phanHoi = new PhanHoi
                {
                    // Gán MaNd nếu > 0, ngược lại là NULL
                    
                    HoTen = model.HoTen,
                    Email = model.Email,
                    SoDienThoai = model.SoDienThoai,
                    NoiDung = model.NoiDung,
                    ThoiGianGui = DateTime.Now,
                    TrangThai = false // Luôn là 'Chưa đọc' khi mới gửi
                };

                // 3. Lưu vào Database
                try
                {
                    _context.PhanHois.Add(phanHoi);
                    _context.SaveChanges();

                    // 4. Thông báo thành công và chuyển hướng (Post-Redirect-Get Pattern)
                    TempData["SuccessMessage"] = "Cảm ơn bạn đã gửi ý kiến! Shop đã nhận được nội dung liên hệ của bạn.";
                }
                catch (Exception ex)
                {
                    // Ghi log lỗi (optional)
                    // Console.WriteLine(ex.Message);
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
