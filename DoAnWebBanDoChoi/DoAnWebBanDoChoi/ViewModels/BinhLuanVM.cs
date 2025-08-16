using System.ComponentModel.DataAnnotations;

namespace DoAnWebBanDoChoi.ViewModels
{
    public class BinhLuanVM
    {
        public int MaSp { get; set; }
        public string TenSanPham { get; set; } = "";
        public string? HinhAnh { get; set; }
        public int MaCtdh { get; set; }
        public int MaDh { get; set; }

        [Range(1, 5, ErrorMessage = "Vui lòng chọn số sao từ 1 đến 5")]
        public int SoSao { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung")]
        public string NoiDung { get; set; } = "";
    }
}
