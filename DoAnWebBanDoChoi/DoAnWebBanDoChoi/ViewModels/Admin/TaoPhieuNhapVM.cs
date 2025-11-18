using System.ComponentModel.DataAnnotations;

namespace DoAnWebBanDoChoi.ViewModels.Admin
{
   
    
    // ViewModel chính cho toàn bộ form tạo phiếu nhập
    public class TaoPhieuNhapVM
    {
        // Thuộc tính của PhieuNhap
        [Required(ErrorMessage = "Vui lòng chọn Nhà cung cấp.")]
        [Display(Name = "Nhà cung cấp")]
        public int MaNcc { get; set; }

        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }
    }
}
