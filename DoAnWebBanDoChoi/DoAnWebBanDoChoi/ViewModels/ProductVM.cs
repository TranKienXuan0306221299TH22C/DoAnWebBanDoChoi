using System.ComponentModel.DataAnnotations;

namespace DoAnWebBanDoChoi.ViewModels
{
    public class ProductVM
    {
        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        [Display(Name = "Tên sản phẩm")]
        public string TenSanPham { get; set; }

        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [Display(Name = "Giá bán")]
        [Required(ErrorMessage = "Vui lòng nhập giá bán")]
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
