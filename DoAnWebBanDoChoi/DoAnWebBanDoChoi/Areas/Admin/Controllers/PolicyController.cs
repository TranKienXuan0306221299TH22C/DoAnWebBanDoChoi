using DoAnWebBanDoChoi.Filters;
using DoAnWebBanDoChoi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace DoAnWebBanDoChoi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class PolicyController : Controller
    {
       
        private readonly AppDbContext _context;

        public PolicyController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var danhSach = _context.Policies.ToList();
            return View(danhSach);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Policy policy)
        {
            if (ModelState.IsValid)
            {
                _context.Policies.Add(policy);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(policy);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var policy = _context.Policies.Find(id);
            if (policy == null) return NotFound();

            return View(policy);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Policy policy)
        {
            if (ModelState.IsValid)
            {
                _context.Policies.Update(policy);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(policy);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var policy = _context.Policies.Find(id);
            if (policy == null) return NotFound();

            return View(policy);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var policy = _context.Policies.Find(id);

            if (policy == null)
            {
                return NotFound();
            }

            _context.Policies.Remove(policy);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();

            var policy = _context.Policies.Find(id);
            if (policy == null) return NotFound();

            return View(policy);
        }
    }
}
