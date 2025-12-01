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
        [Required(ErrorMessage = "Vui lòng nhập giá bán")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá bán phải lớn hơn 0.")]
        public decimal DonGia { get; set; }

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
