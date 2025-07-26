using System;
using System.Collections.Generic;

namespace DoAnWebBanDoChoi.Models;

public partial class PhiVanChuyen
{
    public int MaPvc { get; set; }

    public string TenTinh { get; set; } = null!;

    public string TenHuyen { get; set; } = null!;

    public decimal PhiShip { get; set; }

    public DateTime? NgayTao { get; set; }
}
