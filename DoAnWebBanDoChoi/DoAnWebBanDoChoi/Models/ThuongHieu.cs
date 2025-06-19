using System;
using System.Collections.Generic;

namespace DoAnWebBanDoChoi.Models;

public partial class ThuongHieu
{
    public int MaTh { get; set; }

    public string TenThuongHieu { get; set; } = null!;

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
