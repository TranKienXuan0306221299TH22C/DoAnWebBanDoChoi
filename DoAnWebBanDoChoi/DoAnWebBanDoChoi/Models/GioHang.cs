using System;
using System.Collections.Generic;

namespace DoAnWebBanDoChoi.Models;

public partial class GioHang
{
    public int MaGh { get; set; }

    public int MaNd { get; set; }

    public int MaSp { get; set; }

    public int SoLuong { get; set; }

    public virtual NguoiDung MaNdNavigation { get; set; } = null!;

    public virtual SanPham MaSpNavigation { get; set; } = null!;
}
