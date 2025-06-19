using System;
using System.Collections.Generic;

namespace DoAnWebBanDoChoi.Models;

public partial class NhaCungCap
{
    public int MaNcc { get; set; }

    public string TenNhaCungCap { get; set; } = null!;

    public string DienThoai { get; set; } = null!;

    public string? DiaChi { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
