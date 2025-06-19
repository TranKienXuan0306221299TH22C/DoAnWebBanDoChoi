using System;
using System.Collections.Generic;

namespace DoAnWebBanDoChoi.Models;

public partial class SanPhamYeuThich
{
    public int MaSpyt { get; set; }

    public int MaNd { get; set; }

    public int MaSp { get; set; }

    public virtual NguoiDung MaNdNavigation { get; set; } = null!;

    public virtual SanPham MaSpNavigation { get; set; } = null!;
}
