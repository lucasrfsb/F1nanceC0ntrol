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
    public class FixedCostsController : Controller
    {
        private readonly FinancialDbContext _context;

        public FixedCostsController(FinancialDbContext context)
        {
            _context = context;
        }

        // GET: FixedCosts
        public async Task<IActionResult> Index()
        {
            var costs = await _context.FixedCosts
                .Include(f => f.Category)
                .ToListAsync();
            return View(costs);
        }

        // GET: FixedCosts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cost = await _context.FixedCosts
                .Include(f => f.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cost == null)
            {
                return NotFound();
            }

            return View(cost);
        }

        // GET: FixedCosts/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: FixedCosts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Date,Value,CategoryId")] FixedCost cost)
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

        // GET: FixedCosts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cost = await _context.FixedCosts.FindAsync(id);
            if (cost == null)
            {
                return NotFound();
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", cost.CategoryId);
            return View(cost);
        }

        // POST: FixedCosts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Date,Value,CategoryId")] FixedCost cost)
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

        // GET: FixedCosts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cost = await _context.FixedCosts
                .Include(f => f.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cost == null)
            {
                return NotFound();
            }

            return View(cost);
        }

        // POST: FixedCosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cost = await _context.FixedCosts.FindAsync(id);
            _context.FixedCosts.Remove(cost);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CostExists(int id)
        {
            return _context.FixedCosts.Any(e => e.Id == id);
        }
    }
}