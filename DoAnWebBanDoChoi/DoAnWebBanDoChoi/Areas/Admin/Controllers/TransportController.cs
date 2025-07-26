using Microsoft.AspNetCore.Mvc;
using DoAnWebBanDoChoi.Models;
using System.Linq;

namespace DoAnWebBanDoChoi.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TransportController : Controller
    {
        private readonly AppDbContext _context;

        public TransportController(AppDbContext context)
        {
            _context = context;
        }


        public IActionResult VanChuyen()
        {
            var dsPhi = _context.PhiVanChuyens.OrderByDescending(x => x.NgayTao).ToList();
            return View(dsPhi);
        }


        [HttpPost]
        public IActionResult Them(string TenTinh, string TenHuyen, decimal PhiShip)
        {
            if (string.IsNullOrEmpty(TenTinh) || string.IsNullOrEmpty(TenHuyen) || PhiShip <= 0)
            {
                return BadRequest("Thông tin không hợp lệ.");
            }
            bool daCo = _context.PhiVanChuyens.Any(p =>
                p.TenTinh.ToLower().Trim() == TenTinh.ToLower().Trim() &&
                p.TenHuyen.ToLower().Trim() == TenHuyen.ToLower().Trim()
            );

            if (daCo)
            {
                return Conflict("Địa điểm này đã tồn tại phí vận chuyển.");
            }
            var phi = new PhiVanChuyen
            {
                TenTinh = TenTinh,
                TenHuyen = TenHuyen,
                PhiShip = PhiShip,
                NgayTao = DateTime.Now
            };

            _context.PhiVanChuyens.Add(phi);
            _context.SaveChanges();

            return Ok();
        }
    }
}
