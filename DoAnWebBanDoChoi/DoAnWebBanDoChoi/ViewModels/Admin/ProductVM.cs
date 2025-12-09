using System.ComponentModel.DataAnnotations;

namespace DoAnWebBanDoChoi.ViewModels.Admin
{
    public class ProductVM
    {
        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        [Display(Name = "Tên sản phẩm")]
        public string TenSanPham { get; set; }

        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }
        [Display(Name = "Chất liệu")]
        [Required(ErrorMessage = "Vui lòng nhập chất liệu")]
        public string? ChatLieu { get; set; }

        [Display(Name = "Kích thước")]
        [Required(ErrorMessage = "Vui lòng nhập kích thước")]
        public string? KichThuoc { get; set; }

        [Display(Name = "Độ tuổi phù hợp")]
        [Required(ErrorMessage = "Vui lòng nhập độ tuổi phù hợp")]
        public string? DoTuoiPhuHop { get; set; }

        [Display(Name = "Trọng lượng")]
        public string? TrongLuong { get; set; }
        [Display(Name = "Giá bán")]
        public decimal? DonGia { get; set; } // Dùng decimal?

        // 🎯 BỔ SUNG: Thêm GiaGoc và SoLuong (nếu có trong ProductVM)
        [Display(Name = "Giá gốc")]
        public decimal? GiaGoc { get; set; } // Phải dùng decimal?

        [Display(Name = "Số lượng")]
        public int? SoLuong { get; set; }

        [Display(Name = "Ảnh sản phẩm")]
        public IFormFile? HinhAnhFile { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn thương hiệu")]
        [Display(Name = "Thương hiệu")]
        public int? MaTh { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        [Display(Name = "Danh mục")]
        public int? MaDm { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn nhà cung cấp")]
        [Display(Name = "Nhà cung cấp")]
        public int? MaNcc { get; set; }
    }
}
