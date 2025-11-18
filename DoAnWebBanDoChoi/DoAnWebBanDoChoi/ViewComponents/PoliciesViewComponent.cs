using DoAnWebBanDoChoi.Models;
using Microsoft.AspNetCore.Mvc;

public class PoliciesViewComponent : ViewComponent
{
    private readonly AppDbContext _context;

    public PoliciesViewComponent(AppDbContext context)
    {
        _context = context;
    }

    public IViewComponentResult Invoke()
    {
        var list = _context.Policies.ToList();
        return View(list);
    }
}
