using System;
using System.Collections.Generic;

namespace DoAnWebBanDoChoi.Models;

public partial class BinhLuan
{
    public int MaBl { get; set; }

    public int MaSp { get; set; }

    public int MaNd { get; set; }

    public int Diem { get; set; }

    public string? PhanHoiBinhLuan { get; set; }

    public string? NoiDungBinhLuan { get; set; }

    public int TrangThai { get; set; }

    public DateTime NgayTao { get; set; }

    public virtual NguoiDung MaNdNavigation { get; set; } = null!;

    public virtual SanPham MaSpNavigation { get; set; } = null!;
}
