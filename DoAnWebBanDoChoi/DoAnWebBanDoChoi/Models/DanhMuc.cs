using System;
using System.Collections.Generic;

namespace DoAnWebBanDoChoi.Models;

public partial class DanhMuc
{
    public int MaDm { get; set; }

    public string TenDanhMuc { get; set; } = null!;

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
