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
    public class DailyOperationCostsController : Controller
    {
        private readonly FinancialDbContext _context;

        public DailyOperationCostsController(FinancialDbContext context)
        {
            _context = context;
        }

        // GET: DailyOperationCosts
        public async Task<IActionResult> Index()
        {
            var costs = await _context.DailyOperationCosts
                .Include(d => d.Category)
                .ToListAsync();
            return View(costs);
        }

        // GET: DailyOperationCosts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cost = await _context.DailyOperationCosts
                .Include(d => d.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cost == null)
            {
                return NotFound();
            }

            return View(cost);
        }

        // GET: DailyOperationCosts/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: DailyOperationCosts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Date,Value,CategoryId")] DailyOperationCost cost)
        {
            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                _context.Add(cost);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", cost.CategoryId);
            return View(cost);
        }

        // GET: DailyOperationCosts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cost = await _context.DailyOperationCosts.FindAsync(id);
            if (cost == null)
            {
                return NotFound();
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", cost.CategoryId);
            return View(cost);
        }

        // POST: DailyOperationCosts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Date,Value,CategoryId")] DailyOperationCost cost)
        {
            if (id != cost.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cost);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CostExists(cost.Id))
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

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", cost.CategoryId);
            return View(cost);
        }

        // GET: DailyOperationCosts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cost = await _context.DailyOperationCosts
                .Include(d => d.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cost == null)
            {
                return NotFound();
            }

            return View(cost);
        }

        // POST: DailyOperationCosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cost = await _context.DailyOperationCosts.FindAsync(id);
            _context.DailyOperationCosts.Remove(cost);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CostExists(int id)
        {
            return _context.DailyOperationCosts.Any(e => e.Id == id);
        }
    }
}