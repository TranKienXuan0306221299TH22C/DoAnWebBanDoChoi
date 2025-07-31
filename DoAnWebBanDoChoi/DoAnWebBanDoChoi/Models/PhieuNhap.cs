using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DoAnWebBanDoChoi.Models;

public partial class PhieuNhap
{
    public int MaPn { get; set; }

    public DateTime NgayNhap { get; set; }

    public int NguoiNhap { get; set; }

    public int MaNcc { get; set; }
    [Required(ErrorMessage = "Ghi chú không được bỏ trống")]
    public string? GhiChu { get; set; }

    public int TrangThai { get; set; }

    public virtual ICollection<ChiTietPhieuNhap> ChiTietPhieuNhaps { get; set; } = new List<ChiTietPhieuNhap>();

    public virtual NhaCungCap MaNccNavigation { get; set; } = null!;

    public virtual NguoiDung NguoiNhapNavigation { get; set; } = null!;
}
