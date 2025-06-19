using System;
using System.Collections.Generic;

namespace DoAnWebBanDoChoi.Models;

public partial class DonHang
{
    public int MaDh { get; set; }

    public int MaNd { get; set; }

    public int TrangThai { get; set; }

    public string? GhiChu { get; set; }

    public decimal TongTien { get; set; }

    public DateTime NgayTao { get; set; }

    public DateTime? NgaySua { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual NguoiDung MaNdNavigation { get; set; } = null!;

    public virtual ICollection<ThanhToan> ThanhToans { get; set; } = new List<ThanhToan>();
}
