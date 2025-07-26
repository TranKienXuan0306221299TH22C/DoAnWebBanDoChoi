using DoAnWebBanDoChoi.Filters;
using Microsoft.AspNetCore.Mvc;

namespace DoAnWebBanDoChoi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
