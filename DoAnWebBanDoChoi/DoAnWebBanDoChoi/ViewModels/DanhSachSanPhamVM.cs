
using DoAnWebBanDoChoi.Models;
using X.PagedList;

namespace DoAnWebBanDoChoi.ViewModels
{
    public class DanhSachSanPhamVM
    {
        public IPagedList<SanPham> DanhSachSanPham { get; set; }
        public string? Keyword { get; set; }

        // Thêm các danh sách cho bộ lọc
        public List<ThuongHieu>? DanhSachThuongHieu { get; set; }
        public List<string>? DanhSachDoTuoi { get; set; } // Giả sử bạn có bảng Độ Tuổi
        public List<DanhMuc>? DanhSachDanhMuc { get; set; }

        // Thêm các tham số hiện tại để đánh dấu ô check đã chọn
        public List<int>? SelectedBrands { get; set; }
        public List<int>? SelectedCategories { get; set; }// VD: [1, 5, 8]
        public List<string>? SelectedAges { get; set; } 
    }
}