using DoAnWebBanDoChoi.Helpers;
using DoAnWebBanDoChoi.Models;
using System.Collections.Generic;
using System.Linq; // Cần dùng System.Linq
using Microsoft.EntityFrameworkCore; // Cần dùng Microsoft.EntityFrameworkCore

namespace DoAnWebBanDoChoi.Services.Ham // Namespace của bạn
{

    public interface IFilterService
    {
        IQueryable<SanPham> GetFilteredProducts(
            IQueryable<SanPham> query,
            int? loai,
            
            List<int>? brands,
            List<string>? ages,
            List<int>? selectedCategories,
            string? sort);
    }
    public class FilterService : IFilterService
    {
        public IQueryable<SanPham> GetFilteredProducts(
            IQueryable<SanPham> query,
            int? loai,
            // ĐÃ BỎ: string? keyword,
            List<int>? brands,
            List<string>? ages,
            List<int>? selectedCategories,
            string? sort)
        {
            // 1. Lọc theo DANH MỤC (loai) - CHỈ LỌC NẾU KHÔNG CÓ BỘ LỌC PHỤ
            if (loai.HasValue &&
                (brands == null || !brands.Any()) &&
                (ages == null || !ages.Any()) &&
                (selectedCategories == null || !selectedCategories.Any()))
            {
                query = query.Where(sp => sp.MaDm == loai.Value);
            }

            // ❌ ĐÃ XÓA TOÀN BỘ LOGIC LỌC THEO TỪ KHÓA ❌

            // 2. Lọc theo THƯƠNG HIỆU (brands)
            if (brands != null && brands.Any())
            {
                query = query.Where(sp => brands.Contains(sp.MaTh));
            }

            // 3. Lọc theo ĐỘ TUỔI (ages)
            if (ages != null && ages.Any())
            {
                query = query.Where(sp => sp.DoTuoiPhuHop != null && ages.Contains(sp.DoTuoiPhuHop));
            }

            // 4. Lọc theo DANH MỤC đa chọn (selectedCategories)
            if (selectedCategories != null && selectedCategories.Any())
            {
                query = query.Where(sp => selectedCategories.Contains(sp.MaDm));
            }

            // 5. Logic Sắp xếp (sort) - GIỮ NGUYÊN
            switch (sort)
            {
                case "new":
                    query = query.OrderByDescending(sp => sp.NgayTao);
                    break;
                case "price-asc":
                    query = query.OrderBy(sp => sp.DonGia);
                    break;
                case "price-desc":
                    query = query.OrderByDescending(sp => sp.DonGia);
                    break;
                case "hot":
                    query = query.OrderByDescending(sp => sp.ChiTietDonHangs.Count);
                    break;
                default:
                    query = query.OrderByDescending(sp => sp.MaSp);
                    break;
            }

            return query;
        }
    }
}