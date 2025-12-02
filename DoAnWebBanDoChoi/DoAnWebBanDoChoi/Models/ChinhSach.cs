using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnWebBanDoChoi.Models
{
    [Table("ChinhSach")]
    public partial class ChinhSach
    {
        [Key]
        [Column("MaCS")] // Đảm bảo ánh xạ đúng tên cột trong DB là MaCS
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaCs { get; set; }

        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        [StringLength(50, ErrorMessage = "Tiêu đề không được vượt quá 50 ký tự")] // Quan trọng: Khớp với nvarchar(50)
        public string TieuDe { get; set; } // Bỏ dấu ? vì DB không cho phép Null

        [Required(ErrorMessage = "Nội dung không được để trống")]
        [Column(TypeName = "nvarchar(max)")] // Khớp với nvarchar(MAX)
        public string NoiDung { get; set; } // Bỏ dấu ? vì DB không cho phép Null
    }
}