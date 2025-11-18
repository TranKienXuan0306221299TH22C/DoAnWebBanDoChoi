using Microsoft.EntityFrameworkCore;

namespace DoAnWebBanDoChoi.Models
{
    public class Policy
    {
        public int Id { get; set; }
        public string TenChinhSach { get; set; }
        public string NoiDung { get; set; }
    }
}
