using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnWebBanDoChoi.Models
{
    [Table("CauHinh")]
    public partial class CauHinh
    {
        // Giả định Ten là khóa chính. Nếu bảng có cột Id, hãy chuyển [Key] sang cột Id.
        [Key] // Chuyển Key sang Id
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên không được để trống")]
        [StringLength(150, ErrorMessage = "Tên không được vượt quá 150 ký tự")] // Đã sửa theo DB
        public string Ten { get; set; }

        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        [StringLength(300, ErrorMessage = "Địa chỉ không được vượt quá 300 ký tự")] // Đã sửa theo DB
        public string? DiaChi { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")] // Đã sửa theo DB
        public string? DienThoai { get; set; }
    }
}