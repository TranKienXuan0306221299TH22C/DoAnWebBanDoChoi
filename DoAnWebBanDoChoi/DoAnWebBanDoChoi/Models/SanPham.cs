using System;
using System.Collections.Generic;

namespace DoAnWebBanDoChoi.Models;

public partial class SanPham
{
    public int MaSp { get; set; }

    public string TenSanPham { get; set; } = null!;

    public string? MoTa { get; set; }

    public decimal GiaGoc { get; set; }

    public decimal DonGia { get; set; }

    public int SoLuong { get; set; }

    public string? ChatLieu { get; set; }

    public string? KichThuoc { get; set; }

    public string? DoTuoiPhuHop { get; set; }

    public string? TrongLuong { get; set; }

    public string? HinhAnh { get; set; }

    public int MaTh { get; set; }

    public int MaDm { get; set; }

    public int MaNcc { get; set; }

    public string? Slug { get; set; }

    public int TrangThai { get; set; }

    public DateTime NgayTao { get; set; }

    public DateTime? NgaySua { get; set; }

    public virtual ICollection<BinhLuan> BinhLuans { get; set; } = new List<BinhLuan>();

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual ICollection<ChiTietPhieuNhap> ChiTietPhieuNhaps { get; set; } = new List<ChiTietPhieuNhap>();

    public virtual ICollection<GioHang> GioHangs { get; set; } = new List<GioHang>();

    public virtual DanhMuc MaDmNavigation { get; set; } = null!;

    public virtual NhaCungCap MaNccNavigation { get; set; } = null!;

    public virtual ThuongHieu MaThNavigation { get; set; } = null!;

    public virtual ICollection<SanPhamYeuThich> SanPhamYeuThiches { get; set; } = new List<SanPhamYeuThich>();
}
