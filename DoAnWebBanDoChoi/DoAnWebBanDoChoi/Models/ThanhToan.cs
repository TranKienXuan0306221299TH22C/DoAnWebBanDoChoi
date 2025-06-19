using System;
using System.Collections.Generic;

namespace DoAnWebBanDoChoi.Models;

public partial class ThanhToan
{
    public int MaTt { get; set; }

    public int MaDh { get; set; }

    public int MaNd { get; set; }

    public string PhuongThucThanhToan { get; set; } = null!;

    public string TrangThaiThanhToan { get; set; } = null!;

    public virtual DonHang MaDhNavigation { get; set; } = null!;

    public virtual NguoiDung MaNdNavigation { get; set; } = null!;
}
