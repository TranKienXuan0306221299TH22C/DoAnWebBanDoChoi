using System;
using System.Collections.Generic;

namespace DoAnWebBanDoChoi.Models;

public partial class NguoiDung
{
    public int MaNd { get; set; }

    public string TenDangNhap { get; set; } = null!;

    public string? DienThoai { get; set; }

    public string Email { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public string? HinhAnh { get; set; }

    public string? DiaChi { get; set; }

    public string VaiTro { get; set; } = null!;

    public bool HieuLuc { get; set; }

    public string? RandomKey { get; set; }

    public string? HoTenHienThi { get; set; }

    public virtual ICollection<BinhLuan> BinhLuans { get; set; } = new List<BinhLuan>();

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();

    public virtual ICollection<GioHang> GioHangs { get; set; } = new List<GioHang>();

    public virtual ICollection<SanPhamYeuThich> SanPhamYeuThiches { get; set; } = new List<SanPhamYeuThich>();

    public virtual ICollection<ThanhToan> ThanhToans { get; set; } = new List<ThanhToan>();
}
