using System;
using System.Collections.Generic;

namespace DoAnWebBanDoChoi.Models;

public partial class PhanHoi
{
    public int MaPhanHoi { get; set; }

    public string HoTen { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string SoDienThoai { get; set; } = null!;

    public string TieuDe { get; set; } = null!;

    public string NoiDung { get; set; } = null!;

    public DateTime ThoiGianGui { get; set; }

    public bool? TrangThai { get; set; }
}
