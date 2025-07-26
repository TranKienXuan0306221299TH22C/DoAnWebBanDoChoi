using Microsoft.AspNetCore.Mvc;

namespace DoAnWebBanDoChoi.Controllers
{
    public class DemoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
