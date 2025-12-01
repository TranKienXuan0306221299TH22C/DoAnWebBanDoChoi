using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DoAnWebBanDoChoi.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BinhLuan> BinhLuans { get; set; }

    public virtual DbSet<CauHinh> CauHinhs { get; set; }

    public virtual DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }

    public virtual DbSet<ChiTietPhieuNhap> ChiTietPhieuNhaps { get; set; }

    public virtual DbSet<ChinhSach> ChinhSaches { get; set; }

    public virtual DbSet<DanhMuc> DanhMucs { get; set; }

    public virtual DbSet<DonHang> DonHangs { get; set; }

    public virtual DbSet<GioHang> GioHangs { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<NhaCungCap> NhaCungCaps { get; set; }

    public virtual DbSet<PhanHoi> PhanHois { get; set; }

    public virtual DbSet<PhiVanChuyen> PhiVanChuyens { get; set; }

    public virtual DbSet<PhieuNhap> PhieuNhaps { get; set; }

    public virtual DbSet<SanPham> SanPhams { get; set; }

    public virtual DbSet<SanPhamYeuThich> SanPhamYeuThiches { get; set; }

    public virtual DbSet<ThuongHieu> ThuongHieus { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BinhLuan>(entity =>
        {
            entity.HasKey(e => e.MaBl).HasName("PK__BinhLuan__272475AF6582B93F");

            entity.ToTable("BinhLuan");

            entity.Property(e => e.MaBl).HasColumnName("MaBL");
            entity.Property(e => e.MaNd).HasColumnName("MaND");
            entity.Property(e => e.MaSp).HasColumnName("MaSP");
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NoiDungBinhLuan).HasMaxLength(1000);
            entity.Property(e => e.PhanHoiBinhLuan).HasMaxLength(1000);

            entity.HasOne(d => d.MaNdNavigation).WithMany(p => p.BinhLuans)
                .HasForeignKey(d => d.MaNd)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BinhLuan_NguoiDung");

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.BinhLuans)
                .HasForeignKey(d => d.MaSp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BinhLuan_SanPham");
        });

        modelBuilder.Entity<CauHinh>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("CauHinh");

            entity.Property(e => e.DiaChi).HasMaxLength(300);
            entity.Property(e => e.DienThoai).HasMaxLength(20);
            entity.Property(e => e.Logo).HasMaxLength(255);
            entity.Property(e => e.Ten).HasMaxLength(150);
        });

        modelBuilder.Entity<ChiTietDonHang>(entity =>
        {
            entity.HasKey(e => e.MaCtdh).HasName("PK__ChiTietD__1E4E40F052990D8A");

            entity.ToTable("ChiTietDonHang");

            entity.Property(e => e.MaCtdh).HasColumnName("MaCTDH");
            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.GiaGoc).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.MaDh).HasColumnName("MaDH");
            entity.Property(e => e.MaSp).HasColumnName("MaSP");

            entity.HasOne(d => d.MaDhNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.MaDh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietDonHang_DonHang");

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.MaSp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietDonHang_SanPham");
        });

        modelBuilder.Entity<ChiTietPhieuNhap>(entity =>
        {
            entity.HasKey(e => e.MaCtpn).HasName("PK__ChiTietP__1E4E6075B829A0A2");

            entity.ToTable("ChiTietPhieuNhap");

            entity.Property(e => e.MaCtpn).HasColumnName("MaCTPN");
            entity.Property(e => e.GiaNhap).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.MaPn).HasColumnName("MaPN");
            entity.Property(e => e.MaSp).HasColumnName("MaSP");

            entity.HasOne(d => d.MaPnNavigation).WithMany(p => p.ChiTietPhieuNhaps)
                .HasForeignKey(d => d.MaPn)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietPhieuNhap_PhieuNhap");

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.ChiTietPhieuNhaps)
                .HasForeignKey(d => d.MaSp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietPhieuNhap_SanPham");
        });

        modelBuilder.Entity<ChinhSach>(entity =>
        {
            entity.HasKey(e => e.MaCs);

            entity.ToTable("ChinhSach");

            entity.Property(e => e.MaCs)
                .ValueGeneratedNever()
                .HasColumnName("MaCS");
            entity.Property(e => e.TieuDe).HasMaxLength(50);
        });

        modelBuilder.Entity<DanhMuc>(entity =>
        {
            entity.HasKey(e => e.MaDm).HasName("PK__DanhMuc__2725866EC3B710E1");

            entity.ToTable("DanhMuc");

            entity.Property(e => e.MaDm).HasColumnName("MaDM");
            entity.Property(e => e.TenDanhMuc).HasMaxLength(100);
        });

        modelBuilder.Entity<DonHang>(entity =>
        {
            entity.HasKey(e => e.MaDh).HasName("PK__DonHang__27258661C3075232");

            entity.ToTable("DonHang");

            entity.Property(e => e.MaDh).HasColumnName("MaDH");
            entity.Property(e => e.DiaChi).HasMaxLength(200);
            entity.Property(e => e.DienThoai).HasMaxLength(20);
            entity.Property(e => e.GhiChu).HasMaxLength(500);
            entity.Property(e => e.HoTen).HasMaxLength(50);
            entity.Property(e => e.MaNd).HasColumnName("MaND");
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.PhiVanChuyen).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.PhuongThucThanhToan).HasMaxLength(50);
            entity.Property(e => e.PhuongXa).HasMaxLength(50);
            entity.Property(e => e.QuanHuyen).HasMaxLength(50);
            entity.Property(e => e.TinhThanh).HasMaxLength(50);
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.TrangThai).HasDefaultValue(1);

            entity.HasOne(d => d.MaNdNavigation).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.MaNd)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DonHang_NguoiDung");
        });

        modelBuilder.Entity<GioHang>(entity =>
        {
            entity.HasKey(e => e.MaGh).HasName("PK__GioHang__2725AE85E074EA7D");

            entity.ToTable("GioHang");

            entity.Property(e => e.MaGh).HasColumnName("MaGH");
            entity.Property(e => e.MaNd).HasColumnName("MaND");
            entity.Property(e => e.MaSp).HasColumnName("MaSP");

            entity.HasOne(d => d.MaNdNavigation).WithMany(p => p.GioHangs)
                .HasForeignKey(d => d.MaNd)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GioHang_NguoiDung");

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.GioHangs)
                .HasForeignKey(d => d.MaSp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GioHang_SanPham");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.MaNd).HasName("PK__NguoiDun__2725D724739F69B9");

            entity.ToTable("NguoiDung");

            entity.HasIndex(e => e.Email, "UQ__NguoiDun__A9D10534A8F0618B").IsUnique();

            entity.Property(e => e.MaNd).HasColumnName("MaND");
            entity.Property(e => e.DiaChi).HasMaxLength(200);
            entity.Property(e => e.DienThoai).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.HieuLuc).HasDefaultValue(true);
            entity.Property(e => e.HinhAnh).HasMaxLength(50);
            entity.Property(e => e.HoTenHienThi).HasMaxLength(100);
            entity.Property(e => e.MatKhau).HasMaxLength(100);
            entity.Property(e => e.TenDangNhap).HasMaxLength(100);
            entity.Property(e => e.VaiTro).HasMaxLength(50);
        });

        modelBuilder.Entity<NhaCungCap>(entity =>
        {
            entity.HasKey(e => e.MaNcc).HasName("PK__NhaCungC__3A185DEBB491A59E");

            entity.ToTable("NhaCungCap");

            entity.Property(e => e.MaNcc).HasColumnName("MaNCC");
            entity.Property(e => e.DiaChi).HasMaxLength(300);
            entity.Property(e => e.DienThoai).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(150);
            entity.Property(e => e.TenNhaCungCap).HasMaxLength(150);
        });

        modelBuilder.Entity<PhanHoi>(entity =>
        {
            entity.HasKey(e => e.MaPhanHoi).HasName("PK__PhanHoi__3458D20F666DA7FE");

            entity.ToTable("PhanHoi");

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.SoDienThoai).HasMaxLength(15);
            entity.Property(e => e.ThoiGianGui).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TieuDe).HasMaxLength(100);
            entity.Property(e => e.TrangThai).HasDefaultValue(false);
        });

        modelBuilder.Entity<PhiVanChuyen>(entity =>
        {
            entity.HasKey(e => e.MaPvc).HasName("PK__PhiVanCh__3AE15C9DCBA50543");

            entity.ToTable("PhiVanChuyen");

            entity.Property(e => e.MaPvc).HasColumnName("MaPVC");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PhiShip).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.TenHuyen).HasMaxLength(100);
            entity.Property(e => e.TenTinh).HasMaxLength(100);
        });

        modelBuilder.Entity<PhieuNhap>(entity =>
        {
            entity.HasKey(e => e.MaPn).HasName("PK__PhieuNha__2725E7F0943874E1");

            entity.ToTable("PhieuNhap");

            entity.Property(e => e.MaPn).HasColumnName("MaPN");
            entity.Property(e => e.GhiChu).HasMaxLength(500);
            entity.Property(e => e.MaNcc).HasColumnName("MaNCC");
            entity.Property(e => e.NgayNhap).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaNccNavigation).WithMany(p => p.PhieuNhaps)
                .HasForeignKey(d => d.MaNcc)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PhieuNhap_NhaCungCap");

            entity.HasOne(d => d.NguoiNhapNavigation).WithMany(p => p.PhieuNhaps)
                .HasForeignKey(d => d.NguoiNhap)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PhieuNhap_NguoiDung");
        });

        modelBuilder.Entity<SanPham>(entity =>
        {
            entity.HasKey(e => e.MaSp).HasName("PK__SanPham__2725081C9F752E50");

            entity.ToTable("SanPham");

            entity.HasIndex(e => e.Slug, "UQ__SanPham__BC7B5FB647D27560").IsUnique();

            entity.Property(e => e.MaSp).HasColumnName("MaSP");
            entity.Property(e => e.ChatLieu).HasMaxLength(100);
            entity.Property(e => e.DoTuoiPhuHop).HasMaxLength(100);
            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.GiaGoc).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.HinhAnh).HasMaxLength(100);
            entity.Property(e => e.KichThuoc).HasMaxLength(100);
            entity.Property(e => e.MaDm).HasColumnName("MaDM");
            entity.Property(e => e.MaNcc).HasColumnName("MaNCC");
            entity.Property(e => e.MaTh).HasColumnName("MaTH");
            entity.Property(e => e.MoTa).HasMaxLength(500);
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Slug).HasMaxLength(150);
            entity.Property(e => e.TenSanPham).HasMaxLength(100);
            entity.Property(e => e.TrongLuong).HasMaxLength(100);

            entity.HasOne(d => d.MaDmNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.MaDm)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SanPham_DanhMuc");

            entity.HasOne(d => d.MaNccNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.MaNcc)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SanPham_NhaCungCap");

            entity.HasOne(d => d.MaThNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.MaTh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SanPham_ThuongHieu");
        });

        modelBuilder.Entity<SanPhamYeuThich>(entity =>
        {
            entity.HasKey(e => e.MaSpyt).HasName("PK__SanPhamY__9ED713673EAAA9CB");

            entity.ToTable("SanPhamYeuThich");

            entity.Property(e => e.MaSpyt).HasColumnName("MaSPYT");
            entity.Property(e => e.MaNd).HasColumnName("MaND");
            entity.Property(e => e.MaSp).HasColumnName("MaSP");

            entity.HasOne(d => d.MaNdNavigation).WithMany(p => p.SanPhamYeuThiches)
                .HasForeignKey(d => d.MaNd)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SanPhamYeuThich_NguoiDung");

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.SanPhamYeuThiches)
                .HasForeignKey(d => d.MaSp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SanPhamYeuThich_SanPham");
        });

        modelBuilder.Entity<ThuongHieu>(entity =>
        {
            entity.HasKey(e => e.MaTh).HasName("PK__ThuongHi__27250075A5A3B88E");

            entity.ToTable("ThuongHieu");

            entity.Property(e => e.MaTh).HasColumnName("MaTH");
            entity.Property(e => e.TenThuongHieu).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
