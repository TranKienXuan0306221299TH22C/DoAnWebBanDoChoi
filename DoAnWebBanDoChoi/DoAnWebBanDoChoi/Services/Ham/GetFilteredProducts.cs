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
            string? keyword,
            List<int>? brands,
            List<string>? ages,
            List<int>? selectedCategories,
            string? sort);
    }
    public class FilterService : IFilterService
    {
        // 🚩 Phương thức GetFilteredProducts PHẢI NẰM TRONG CLASS NÀY 🚩
        public IQueryable<SanPham> GetFilteredProducts(
            IQueryable<SanPham> query,
            int? loai,
            string? keyword,
            List<int>? brands,
            List<string>? ages,
            List<int>? selectedCategories,
            string? sort)
        {
            // 1. Lọc theo DANH MỤC (loai) - Chỉ áp dụng nếu có loai
            if (loai.HasValue)
            {
                query = query.Where(sp => sp.MaDm == loai.Value);
            }

            // 2. Lọc theo TỪ KHÓA (keyword) - Chỉ áp dụng nếu có keyword
            if (!string.IsNullOrEmpty(keyword))
            {
                var keywordUnsign = StringHelper.ToUnsign(keyword).ToLower();
                var keywordSigned = keyword.ToLower();

                // 🚩 THỰC HIỆN LỌC TRÊN CLIENT (Sử dụng ToList() an toàn hơn khi có hàm không dấu) 🚩
                var list = query.ToList();

                // Áp dụng lọc trên bộ nhớ (client-side)
                query = list.Where(sp =>
                             // Lọc theo ký tự có dấu
                             (sp.TenSanPham != null && sp.TenSanPham.ToLower().Contains(keywordSigned)) ||
                             // Lọc theo ký tự không dấu
                             (sp.TenSanPham != null && StringHelper.ToUnsign(sp.TenSanPham).ToLower().Contains(keywordUnsign))
                         ).AsQueryable(); // Chuyển lại thành IQueryable
            }

            // 3. Lọc theo THƯƠNG HIỆU (brands)
            if (brands != null && brands.Any())
            {
                query = query.Where(sp => brands.Contains(sp.MaTh));
            }

            // 4. Lọc theo ĐỘ TUỔI (ages)
            if (ages != null && ages.Any())
            {
                query = query.Where(sp => sp.DoTuoiPhuHop != null && ages.Contains(sp.DoTuoiPhuHop));
            }
            if (selectedCategories != null && selectedCategories.Any())
            {
                query = query.Where(sp => selectedCategories.Contains(sp.MaDm));
            }
            // 5. Logic Sắp xếp (sort)
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