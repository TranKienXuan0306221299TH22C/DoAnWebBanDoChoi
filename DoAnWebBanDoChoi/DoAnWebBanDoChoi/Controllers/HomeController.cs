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
            _filterService = filterService; // Gán
        }
        [Route("/404")]
        public IActionResult Loi()
        {
            return View();
        }

        //public IActionResult Index(int? loai, string? sort, int page = 1)
        //{
        //    var query = _context.SanPhams
        //        .Include(sp => sp.MaDmNavigation)
        //        .Where(sp => !loai.HasValue || sp.MaDm == loai.Value);
        //        //.OrderByDescending(sp => sp.MaSp);
        //    switch (sort)
        //    {
        //        case "new":
        //            query = query.OrderByDescending(sp => sp.NgayTao);
        //            break;

        //        case "price-asc":
        //            query = query.OrderBy(sp => sp.DonGia);
        //            break;

        //        case "price-desc":
        //            query = query.OrderByDescending(sp => sp.DonGia);
        //            break;

        //        case "hot":
        //            // Giả sử bán chạy dựa vào số lượng bán (phải tính từ ChiTietDonHang nếu có)
        //            query = query.OrderByDescending(sp => sp.ChiTietDonHangs.Count);
        //            break;
        //        case "favorite":
        //            query = query
        //                .Where(sp => sp.SanPhamYeuThiches.Any()) // chỉ lấy sản phẩm có yêu thích
        //                .OrderByDescending(sp => sp.SanPhamYeuThiches.Count); // sắp theo số lượt
        //            break;
        //        case "all":
        //            // không sắp xếp gì cả — cứ lấy mặc định từ CSDL
        //            break;
        //        default:
        //            query = query.OrderBy(sp => sp.MaSp);  // Mặc định
        //            break;
        //    }
        //    int pageSize = 6;
        //    var danhSach = query.ToPagedList(page, pageSize);

        //    var sanPhamMoi = _context.SanPhams
        //        .OrderByDescending(sp => sp.NgayTao)
        //        .Take(6)
        //        .ToList();

        //    var model = new TrangChuVM
        //    {
        //        DanhSachSanPham = danhSach,
        //        SanPhamMoi = sanPhamMoi
        //    };

        //    return View(model);
        //}
        // Trong HomeController.cs

        // 1. Chỉ giữ lại logic cơ bản cho Trang Chủ
        // Loai bỏ tham số 'loai' và 'sort'
        // Trong HomeController.cs
        public IActionResult Index(int page = 1) // Chỉ cần tham số phân trang 'page'
        {
            // Lấy danh sách sản phẩm MỚI nhất (dùng cho Carousel, giữ nguyên)
            var sanPhamMoi = _context.SanPhams
                .OrderByDescending(sp => sp.NgayTao)
                .Take(6)
                .ToList();

            // 🚩 Logic Lấy TẤT CẢ sản phẩm và áp dụng Phân Trang 🚩
            int pageSize = 6;

            var query = _context.SanPhams
                // Sắp xếp mặc định cho trang chủ (ví dụ: theo MaSp hoặc NgayTao)
                .OrderByDescending(sp => sp.MaSp);

            // Áp dụng Phân Trang
            var danhSach = query.ToPagedList(page, pageSize);

            var model = new TrangChuVM
            {
                // Gán danh sách sản phẩm có phân trang
                DanhSachSanPham = danhSach,
                SanPhamMoi = sanPhamMoi
            };

            return View(model);
        }
        // Trong HomeController.cs, sửa Action Search

        // Thay vì return View("Index", model);
        
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
       
            int pageSize = 3;
            //int pageNumber = page ?? 1;
            int pageNumber = Math.Max(page ?? 1, 1);
            var binhLuan = _context.BinhLuans
                .Include(bl => bl.MaNdNavigation)
                .Include(bl => bl.MaSpNavigation)
                .Where(bl => bl.MaSp == id )
                .OrderByDescending(bl => bl.NgayTao)
                .ToPagedList(pageNumber, pageSize);

            // Lấy sản phẩm liên quan (cùng danh mục, khác sản phẩm hiện tại)
            var sanPhamLienQuan = _context.SanPhams
                .Where(sp => sp.MaDm == sanPham.MaDm && sp.MaSp != sanPham.MaSp && sp.TrangThai == 1)
                .OrderByDescending(sp => sp.NgayTao) // mới nhất
                .Take(4) // giới hạn 4 sản phẩm
                .ToList();

            var model = new ChiTietSanPhamVM
            {
                
                SanPham = sanPham,
                BinhLuans = binhLuan,
                SanPhamLienQuan = sanPhamLienQuan
            };

            return View(model);
        }

        // Trong HomeController.cs

        // Thêm một thuộc tính cho ViewModel để lưu Tên Danh mục/Từ khóa
        // Trong HomeController.cs

        // Tùy chọn: Sử dụng routing thân thiện hơn (vd: /danh-muc/2)

        //public IActionResult Category(int? loai, string? sort, int page = 1)
        //{
        //    // 1. Khởi tạo biến query ngay từ đầu
        //    // Bắt đầu bằng cách lấy tất cả sản phẩm
        //    IQueryable<SanPham> query = _context.SanPhams
        //                                .Include(sp => sp.MaDmNavigation)
        //                                .Where(sp => sp.TrangThai == 1); // Chỉ lấy sản phẩm đang hoạt động

        //    // 2. Logic Lọc theo Danh mục (loai)
        //    string tieuDe = "Tất cả sản phẩm";
        //    if (loai.HasValue)
        //    {
        //        var dm = _context.DanhMucs.SingleOrDefault(d => d.MaDm == loai.Value);
        //        if (dm != null)
        //        {
        //            tieuDe = dm.TenDanhMuc;
        //        }

        //        // Thêm điều kiện lọc vào query nếu có 'loai'
        //        query = query.Where(sp => sp.MaDm == loai.Value);
        //    }

        //    // Lưu lại thông tin để dùng trong View (tiêu đề, phân trang)
        //    ViewData["CurrentSort"] = sort;
        //    ViewData["loai"] = loai;

        //    // 3. Logic Sắp xếp (sort)
        //    switch (sort)
        //    {
        //        case "new":
        //            query = query.OrderByDescending(sp => sp.NgayTao);
        //            break;
        //        case "price-asc":
        //            query = query.OrderBy(sp => sp.DonGia);
        //            break;
        //        case "price-desc":
        //            query = query.OrderByDescending(sp => sp.DonGia);
        //            break;
        //        case "hot":
        //            // Nếu bạn có cột bán chạy (SoldCount) trong DB, dùng nó
        //            // Nếu không, có thể sắp xếp theo lượt mua hàng hoặc giữ nguyên
        //            query = query.OrderByDescending(sp => sp.ChiTietDonHangs.Count());
        //            break;
        //        default:
        //            query = query.OrderByDescending(sp => sp.MaSp); // Mặc định
        //            break;
        //    }

        //    // 4. Áp dụng Phân Trang và tạo Model
        //    int pageSize = 6;
        //    // Bây giờ 'query' đã được khai báo và gán giá trị, nên .ToPagedList() sẽ hoạt động
        //    var danhSach = query.ToPagedList(page, pageSize);

        //    var model = new TrangChuVM
        //    {
        //        DanhSachSanPham = danhSach,
        //        Keyword = tieuDe // Dùng Keyword để hiển thị Tiêu đề cho trang Danh mục
        //    };

        //    // 5. Trả về View Search (View chung)
        //    return View("Search", model);
        //}
        // Trong HomeController.cs

        //public IActionResult Search(string keyword, int page = 1, int pageSize = 8)
        //{
        //    if (string.IsNullOrWhiteSpace(keyword))
        //    {
        //        // Chuyển hướng về trang Category (nếu không có từ khóa thì coi như xem tất cả)
        //        return RedirectToAction("Category");
        //    }

        //    // Logic tìm kiếm (giữ nguyên)
        //    var keywordUnsign = StringHelper.ToUnsign(keyword).ToLower();
        //    var keywordSigned = keyword.ToLower();

        //    var query = _context.SanPhams
        //        .Where(sp => sp.TrangThai == 1)
        //        .AsEnumerable()
        //        .Where(sp =>
        //            sp.TenSanPham.ToLower().Contains(keywordSigned) ||
        //            StringHelper.ToUnsign(sp.TenSanPham).ToLower().Contains(keywordUnsign)
        //        )
        //        .OrderByDescending(sp => sp.MaSp);

        //    var danhSach = query.ToPagedList(page, pageSize);

        //    // Tạo ViewModel đơn giản
        //    var model = new DanhSachSanPhamVM // Dùng lại ViewModel này
        //    {
        //        DanhSachSanPham = danhSach,
        //        Keyword = keyword // Thêm Keyword để hiển thị trên View
        //    };

        //    // 🚩 ĐIỂM QUAN TRỌNG: Trả về View "Search" (View mới bạn sẽ tạo)
        //    // Không dùng View("Index", model) nữa
        //    return View("Search", model);
        //}
        // Trong HomeController.cs
        public IActionResult Search(
    string keyword,
    int page = 1,
    int pageSize = 8,
    string? sort = null,
    [FromQuery] List<int>? brands = null,
    [FromQuery] List<string>? ages = null,
    [FromQuery] List<int>? selectedCategories = null) // Nhận tham số
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return RedirectToAction("Category");
            }

            IQueryable<SanPham> baseQuery = _context.SanPhams
                                             .Include(sp => sp.MaDmNavigation)
                                             .Include(sp => sp.MaThNavigation)
                                             .Where(sp => sp.TrangThai == 1);

            var query = _filterService.GetFilteredProducts(baseQuery, null, keyword, brands, ages, selectedCategories, sort);

            var danhSach = query.ToPagedList(page, pageSize);

            var danhSachDoTuoiDocNhat = _context.SanPhams
                                                .Select(sp => sp.DoTuoiPhuHop)
                                                .Where(dt => !string.IsNullOrEmpty(dt))
                                                .Distinct()
                                                .ToList();

            var danhSachThuongHieu = _context.ThuongHieus.ToList();
            var danhSachDanhMuc = _context.DanhMucs.ToList();


            string tieuDe = $"Kết quả tìm kiếm cho: {keyword}";

            var model = new DanhSachSanPhamVM
            {
                DanhSachSanPham = danhSach,
                Keyword = tieuDe,
                DanhSachThuongHieu = danhSachThuongHieu,
                DanhSachDanhMuc = danhSachDanhMuc,
                DanhSachDoTuoi = danhSachDoTuoiDocNhat, // SỬ DỤNG ĐÚNG TÊN THUỘC TÍNH BẠN ĐÃ CUNG CẤP

                SelectedBrands = brands,
                SelectedAges = ages,
                SelectedCategories = selectedCategories // 🚩 ĐÃ THÊM THAM SỐ VÀO MODEL 🚩
            };

            ViewData["CurrentSort"] = sort;

            return View("Search", model);
        }

        // -----------------------------------------------------------------------
        // --- ACTION CATEGORY (ĐÃ SỬA TÊN THUỘC TÍNH) ---
        // -----------------------------------------------------------------------
        [Route("danh-muc/{loai:int?}")]
        public IActionResult Category(
            int? loai,
            string? sort,
            int page = 1,
            List<int>? brands = null,
            List<string>? ages = null,
            [FromQuery] List<int>? selectedCategories = null)
        {
            // 1. Khởi tạo Query cơ bản
            IQueryable<SanPham> baseQuery = _context.SanPhams
                                             .Include(sp => sp.MaDmNavigation)
                                             .Include(sp => sp.MaThNavigation)
                                             .Where(sp => sp.TrangThai == 1);

            // 2. GỌI HÀM SỬ DỤNG SERVICE
            // 🚩 XÓA BỘ LỌC CHÍNH (loai) NẾU CÓ BỘ LỌC PHỤ SELECTEDCATEGORIES 🚩
            int? loaiToFilter = loai;
            if (selectedCategories != null && selectedCategories.Any())
            {
                // Nếu người dùng đã chọn danh mục từ bộ lọc phụ (checkbox), thì bỏ qua loai chính.
                loaiToFilter = null;
            }


            var query = _filterService.GetFilteredProducts(baseQuery, loaiToFilter, null, brands, ages, selectedCategories, sort);
            //
            // 3. Áp dụng Phân Trang
            int pageSize = 6;
            var danhSach = query.ToPagedList(page, pageSize);

            // 4. Lấy dữ liệu cho các tùy chọn bộ lọc
            var danhSachDoTuoiDocNhat = _context.SanPhams
                                                .Select(sp => sp.DoTuoiPhuHop)
                                                .Where(dt => !string.IsNullOrEmpty(dt))
                                                .Distinct()
                                                .ToList();

            var danhSachThuongHieu = _context.ThuongHieus.ToList();
            var danhSachDanhMuc = _context.DanhMucs.ToList();

            // 5. Xác định Tiêu đề
            string tieuDe = "Tất cả sản phẩm";
            if (loai.HasValue)
            {
                var dm = _context.DanhMucs.SingleOrDefault(d => d.MaDm == loai.Value);
                if (dm != null)
                {
                    tieuDe = dm.TenDanhMuc; // SỬ DỤNG ĐÚNG TÊN THUỘC TÍNH BẠN ĐÃ CUNG CẤP
                }
            }

            // 6. KHAI BÁO VÀ TẠO MODEL
            var model = new DanhSachSanPhamVM
            {
                DanhSachSanPham = danhSach,
                Keyword = tieuDe,

                DanhSachThuongHieu = danhSachThuongHieu,
                DanhSachDanhMuc = danhSachDanhMuc,
                DanhSachDoTuoi = danhSachDoTuoiDocNhat, // SỬ DỤNG ĐÚNG TÊN THUỘC TÍNH BẠN ĐÃ CUNG CẤP

                SelectedBrands = brands,
                SelectedAges = ages,
                SelectedCategories = selectedCategories // ĐÃ THÊM THAM SỐ VÀO MODEL
            };

            ViewData["CurrentSort"] = sort;
            ViewData["loai"] = loai;

            return View("Search", model);
        }
        public IActionResult Search2(string keyword, int page = 1, int pageSize = 8)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return RedirectToAction("Index");
            }

            var keywordUnsign = StringHelper.ToUnsign(keyword).ToLower();
            var keywordSigned = keyword.ToLower();

            var query = _context.SanPhams
                .Where(sp => sp.TrangThai == 1)
                .AsEnumerable()
                .Where(sp =>
                    sp.TenSanPham.ToLower().Contains(keywordSigned) ||
                    StringHelper.ToUnsign(sp.TenSanPham).ToLower().Contains(keywordUnsign)
                )
                .OrderByDescending(sp => sp.MaSp);

            var danhSach = query.ToPagedList(page, pageSize);

            var sanPhamMoi = _context.SanPhams
                .OrderByDescending(sp => sp.NgayTao)
                .Take(6)
                .ToList();

            var model = new TrangChuVM
            {
                DanhSachSanPham = danhSach,
                SanPhamMoi = sanPhamMoi
            };
            return View("Index", model);
        }
    }
}
