using System;
using System.Collections.Generic;

namespace DoAnWebBanDoChoi.Models;

public partial class KhuyenMai
{
    public int MaKm { get; set; }

    public string TenKhuyenMai { get; set; } = null!;

    public int PhanTram { get; set; }

    public DateTime NgayBatDau { get; set; }

    public DateTime? NgayKetThuc { get; set; }

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
