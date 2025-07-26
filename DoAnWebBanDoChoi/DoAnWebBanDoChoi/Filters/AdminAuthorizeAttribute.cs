using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace DoAnWebBanDoChoi.Filters
{
    public class AdminAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var vaiTro = session.GetString("VaiTro");

            if (string.IsNullOrEmpty(vaiTro) || (vaiTro != "admin" && vaiTro != "nhanvien"))
            {
                context.Result = new RedirectToActionResult("Login", "Account", new { area = "" });
            }
        }
    }
}