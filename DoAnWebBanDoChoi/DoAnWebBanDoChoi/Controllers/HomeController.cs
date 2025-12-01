using DoAnWebBanDoChoi.Helpers;
using DoAnWebBanDoChoi.Models;
using DoAnWebBanDoChoi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using X.PagedList.Extensions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using DoAnWebBanDoChoi.Services.Ham;

namespace DoAnWebBanDoChoi.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IFilterService _filterService;
        public HomeController(AppDbContext context, IFilterService filterService)
        {
            _context = context;
            _filterService = filterService; 
        }
        [Route("/404")]
        public IActionResult Loi()
        {
            return View();
        }

        
        public IActionResult Index(int page = 1) // Chỉ cần tham số phân trang 'page'
        {
            // Lấy danh sách sản phẩm MỚI nhất (dùng cho Carousel, giữ nguyên)
            // 💡 Thêm điều kiện Trạng thái == 1 ở đây nếu bạn muốn Carousel cũng chỉ hiện sp đang bán
            var sanPhamMoi = _context.SanPhams
                .Where(sp => sp.TrangThai == 1) // **ĐIỀU KIỆN MỚI CHO SẢN PHẨM MỚI**
                .OrderByDescending(sp => sp.NgayTao)
                .Take(6)
                .ToList();

            // 🚩 Logic Lấy TẤT CẢ sản phẩm và áp dụng Phân Trang 🚩
            int pageSize = 6;

            var query = _context.SanPhams
                // **THÊM ĐIỀU KIỆN TRẠNG THÁI == 1 Ở ĐÂY**
                .Where(sp => sp.TrangThai == 1)
                // Sắp xếp mặc định cho trang chủ (ví dụ: theo MaSp hoặc NgayTao)
                .OrderBy(sp => sp.MaSp);

            // Áp dụng Phân Trang
            // Lưu ý: Đảm bảo đã cài đặt thư viện 'X.PagedList' hoặc tương đương.
            var danhSach = query.ToPagedList(page, pageSize);

            var model = new TrangChuVM
            {
                // Gán danh sách sản phẩm có phân trang
                DanhSachSanPham = danhSach,
                SanPhamMoi = sanPhamMoi
            };

            return View(model);
        }
  


        [Route("san-pham/{slug}-{id}")]
        public IActionResult Detail(string slug, int id, int? page)
        {
            var sanPham = _context.SanPhams
                .Include(sp => sp.MaDmNavigation)
                .Include(sp => sp.MaThNavigation)
                .SingleOrDefault(sp => sp.MaSp == id && sp.TrangThai == 1);

            if (sanPham == null)
                return Redirect("/404");

            if (sanPham.Slug != slug)
            {
                return RedirectToAction("Detail", new { slug = sanPham.Slug, id = sanPham.MaSp });
            }

            
            var queryBinhLuan = _context.BinhLuans
        .Where(bl => bl.MaSp == id);

            
            double diemTB = 0.0;
            if (queryBinhLuan.Any()) 
            {
                
                diemTB = queryBinhLuan.Average(bl => (double)bl.Diem);
            }
            else
            {
                diemTB = 5.0; 
            }

            
            int pageSize = 3;
            int pageNumber = Math.Max(page ?? 1, 1);

            var binhLuanPhanTrang = queryBinhLuan
                .Include(bl => bl.MaNdNavigation)
                .Include(bl => bl.MaSpNavigation)
                .OrderByDescending(bl => bl.NgayTao)
                .ToPagedList(pageNumber, pageSize); 

            
            var sanPhamLienQuan = _context.SanPhams
                .Where(sp => sp.MaDm == sanPham.MaDm && sp.MaSp != sanPham.MaSp && sp.TrangThai == 1)
                .OrderByDescending(sp => sp.NgayTao) // mới nhất
                .Take(4) // giới hạn 4 sản phẩm
                .ToList();

            var model = new ChiTietSanPhamVM
            {
                SanPham = sanPham,
                BinhLuans = binhLuanPhanTrang, // BinhLuan đã được phân trang đúng
                SanPhamLienQuan = sanPhamLienQuan,
                DiemSaoTrungBinh = diemTB // Gán điểm đã tính
            };

            return View(model);
        }
       
        public IActionResult Search(
            string keyword,
            int page = 1,
            int pageSize = 8, // Đã đặt lại về 8
            string? sort = null,
            [FromQuery] List<int>? brands = null,
            [FromQuery] List<string>? ages = null,
            [FromQuery] List<int>? selectedCategories = null)
        {
            if (string.IsNullOrWhiteSpace(keyword) &&
                (brands == null || !brands.Any()) &&
                (ages == null || !ages.Any()) &&
                (selectedCategories == null || !selectedCategories.Any()))
            {
                // Nếu không có cả keyword và bộ lọc, chuyển về trang Category
                return RedirectToAction("Category");
            }

            IQueryable<SanPham> baseQuery = _context.SanPhams
                                                    .Include(sp => sp.MaDmNavigation)
                                                    .Include(sp => sp.MaThNavigation)
                                                    .Where(sp => sp.TrangThai == 1);

            // 1. Áp dụng LỌC THEO TỪ KHÓA (keyword) NGAY TRONG CONTROLLER
            if (!string.IsNullOrEmpty(keyword))
            {
                var keywordUnsign = StringHelper.ToUnsign(keyword).ToLower();
                var keywordSigned = keyword.ToLower();

                // Load TẤT CẢ sản phẩm đủ điều kiện (Trạng thái = 1) vào bộ nhớ
                var allProducts = baseQuery.ToList();

                // Áp dụng lọc từ khóa trên bộ nhớ
                baseQuery = allProducts.Where(sp =>
                                    (sp.TenSanPham != null && sp.TenSanPham.ToLower().Contains(keywordSigned)) ||
                                    (sp.TenSanPham != null && StringHelper.ToUnsign(sp.TenSanPham).ToLower().Contains(keywordUnsign))
                                ).AsQueryable(); // Trả lại thành IQueryable
            }

            // 2. Chuyển kết quả baseQuery đã lọc theo Từ khóa cho Service
            // Service chỉ áp dụng Brand, Age, Category đa chọn và Sort (loai = null)
            var query = _filterService.GetFilteredProducts(
                baseQuery,
                null, // loaiToFilter LUÔN LUÔN là null khi đã có keyword (không giới hạn phạm vi search)
                brands,
                ages,
                selectedCategories,
                sort
            );

           
            var danhSach = query.ToPagedList(page, pageSize);

            // 3. Lấy dữ liệu cho các tùy chọn bộ lọc (Giữ nguyên)
            var danhSachDoTuoiDocNhat = _context.SanPhams.Select(sp => sp.DoTuoiPhuHop).Where(dt => !string.IsNullOrEmpty(dt)).Distinct().ToList();
            var danhSachThuongHieu = _context.ThuongHieus.ToList();
            var danhSachDanhMuc = _context.DanhMucs.ToList();


            string tieuDe = string.IsNullOrEmpty(keyword) ? "Tất cả sản phẩm" : $"Kết quả tìm kiếm cho: {keyword}";

            var model = new DanhSachSanPhamVM
            {
                DanhSachSanPham = danhSach,
                Keyword = tieuDe,
                DanhSachThuongHieu = danhSachThuongHieu,
                DanhSachDanhMuc = danhSachDanhMuc,
                DanhSachDoTuoi = danhSachDoTuoiDocNhat,

                SelectedBrands = brands,
                SelectedAges = ages,
                SelectedCategories = selectedCategories
            };

            ViewData["CurrentSort"] = sort;

            return View("Search", model);
        }
       
        [Route("danh-muc/{loai:int?}")]
        public IActionResult Category(
            int? loai,
            string? sort,
            int page = 1,
            int pageSize = 8, // Đã đặt lại về 8
            List<int>? brands = null,
            List<string>? ages = null,
            [FromQuery] List<int>? selectedCategories = null)
        {
            // 1. Khởi tạo Query cơ bản (Không lọc theo loai ở đây nữa)
            IQueryable<SanPham> baseQuery = _context.SanPhams
                                                    .Include(sp => sp.MaDmNavigation)
                                                    .Include(sp => sp.MaThNavigation)
                                                    .Where(sp => sp.TrangThai == 1);

            // 2. GỌI HÀM SỬ DỤNG SERVICE
            // loaiToFilter = loai (Chỉ để Service biết loai nào là loai chính nếu không có bộ lọc phụ)
            int? loaiToFilter = loai;

            // 🚩 ĐÃ SỬA: Lỗi CS1501 được khắc phục 🚩
            var query = _filterService.GetFilteredProducts(
                baseQuery,
                loaiToFilter, // Truyền loai chính vào để Service quyết định lọc hay không
                brands,
                ages,
                selectedCategories,
                sort
            );

            var danhSach = query.ToPagedList(page, pageSize);

            // 3. Lấy dữ liệu cho các tùy chọn bộ lọc (Giữ nguyên)
            var danhSachDoTuoiDocNhat = _context.SanPhams.Select(sp => sp.DoTuoiPhuHop).Where(dt => !string.IsNullOrEmpty(dt)).Distinct().ToList();
            var danhSachThuongHieu = _context.ThuongHieus.ToList();
            var danhSachDanhMuc = _context.DanhMucs.ToList();

            // 4. Xác định Tiêu đề (Giữ nguyên)
            string tieuDe = "Tất cả sản phẩm";
            if (loai.HasValue)
            {
                var dm = _context.DanhMucs.SingleOrDefault(d => d.MaDm == loai.Value);
                if (dm != null)
                {
                    tieuDe = dm.TenDanhMuc;
                }
            }

            var model = new DanhSachSanPhamVM
            {
                DanhSachSanPham = danhSach,
                Keyword = tieuDe, 
                DanhSachThuongHieu = danhSachThuongHieu,
                DanhSachDanhMuc = danhSachDanhMuc,
                DanhSachDoTuoi = danhSachDoTuoiDocNhat,
                SelectedBrands = brands,
                SelectedAges = ages,
                SelectedCategories = selectedCategories
            };

            ViewData["CurrentSort"] = sort;
            ViewData["loai"] = loai;

            return View("Search", model);
        }
    }
}
