using DoAnWebBanDoChoi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class PolicyController : Controller
{
    private readonly AppDbContext _context;

    public PolicyController(AppDbContext context)
    {
        _context = context;
    }

    // Hiển thị chi tiết chính sách
    public IActionResult Detail(int id)
    {
        var policy = _context.Policies.FirstOrDefault(p => p.Id == id);

        if (policy == null)
            return NotFound();

        return View(policy);
    }
}
