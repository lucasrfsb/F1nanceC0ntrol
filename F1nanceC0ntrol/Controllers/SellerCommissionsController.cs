using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using F1nanceC0ntrol.Data;
using F1nanceC0ntrol.Models;

namespace F1nanceC0ntrol.Controllers
{
    public class SellerCommissionsController : Controller
    {
        private readonly FinancialDbContext _context;

        public SellerCommissionsController(FinancialDbContext context)
        {
            _context = context;
        }

        // GET: SellerCommissions
        public async Task<IActionResult> Index()
        {
            var commissions = await _context.SellerCommissions
                .Include(s => s.Category)
                .ToListAsync();
            return View(commissions);
        }

        // GET: SellerCommissions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var commission = await _context.SellerCommissions
                .Include(s => s.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (commission == null)
            {
                return NotFound();
            }

            return View(commission);
        }

        // GET: SellerCommissions/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = _context.Categories
            .Where(c => c.Name == "Comissões")
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name

            })
            .ToList();
                return View();
        }

        // POST: SellerCommissions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Date,Value,CategoryId,EmployeeName")] SellerCommission commission)
        {
            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                _context.Add(commission);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = _context.Categories
            .Where(c => c.Name == "Comissões")
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name

            })
            .ToList(); return View(commission);
        }

        // GET: SellerCommissions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var commission = await _context.SellerCommissions.FindAsync(id);
            if (commission == null)
            {
                return NotFound();
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", commission.CategoryId);
            return View(commission);
        }

        // POST: SellerCommissions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Date,Value,CategoryId,EmployeeName")] SellerCommission commission)
        {
            if (id != commission.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(commission);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommissionExists(commission.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", commission.CategoryId);
            return View(commission);
        }

        // GET: SellerCommissions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var commission = await _context.SellerCommissions
                .Include(s => s.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (commission == null)
            {
                return NotFound();
            }

            return View(commission);
        }

        // POST: SellerCommissions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var commission = await _context.SellerCommissions.FindAsync(id);
            _context.SellerCommissions.Remove(commission);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommissionExists(int id)
        {
            return _context.SellerCommissions.Any(e => e.Id == id);
        }
    }
}