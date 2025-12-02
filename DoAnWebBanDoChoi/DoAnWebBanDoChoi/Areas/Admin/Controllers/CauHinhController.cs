using DoAnWebBanDoChoi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAnWebBanDoChoi.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CauHinhController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment; // Dùng để lấy đường dẫn lưu ảnh

        public CauHinhController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Admin/CauHinh
        public async Task<IActionResult> Index()
        {
            // Lấy bản ghi cấu hình đầu tiên
            var cauHinh = await _context.CauHinhs.FirstOrDefaultAsync();

            // Nếu chưa có dữ liệu, tạo một bản ghi rỗng để người dùng nhập
            if (cauHinh == null)
            {
                cauHinh = new CauHinh();
                // Lưu tạm để có dữ liệu
                cauHinh.Ten = "Website Đồ Chơi"; // Giá trị mặc định
                cauHinh.DiaChi = "Chưa cập nhật";
                cauHinh.DienThoai = "0000000000";
                _context.Add(cauHinh);
                await _context.SaveChangesAsync();
            }

            return View(cauHinh);
        }

        // GET: Hiển thị form để sửa dữ liệu
        public async Task<IActionResult> Edit()
        {
            var cauHinh = await _context.CauHinhs.FirstOrDefaultAsync();
            if (cauHinh == null) return NotFound();
            return View(cauHinh);
        }

        // POST: Admin/CauHinh/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CauHinh model)
        {
            if (ModelState.IsValid)
            {
                // Lấy dữ liệu cũ từ DB ra để cập nhật
                // Lưu ý: Vì Ten là Key, nếu sửa Ten thì EF sẽ hiểu là thêm mới. 
                // Tốt nhất nên dùng Id làm Key. Ở đây ta xử lý theo hướng update dòng đầu tiên.
                var cauHinhDb = await _context.CauHinhs.FirstOrDefaultAsync();

                if (cauHinhDb == null) return NotFound();

                // Cập nhật thông tin text
                cauHinhDb.Ten = model.Ten;
                cauHinhDb.DiaChi = model.DiaChi;
                cauHinhDb.DienThoai = model.DienThoai;

                _context.Update(cauHinhDb);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Cập nhật cấu hình thành công!";
                return RedirectToAction(nameof(Index));
            }

            return View(nameof(Index), model);
        }
    }
}