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
    public class AfterSaleCostsController : Controller
    {
        private readonly FinancialDbContext _context;

        public AfterSaleCostsController(FinancialDbContext context)
        {
            _context = context;
        }

        // GET: AfterSaleCosts
        public async Task<IActionResult> Index()
        {
            var costs = await _context.AfterSaleCosts
                .Include(a => a.Category)
                .ToListAsync();
            return View(costs);
        }

        // GET: AfterSaleCosts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cost = await _context.AfterSaleCosts
                .Include(a => a.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cost == null)
            {
                return NotFound();
            }

            return View(cost);
        }

        // GET: AfterSaleCosts/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: AfterSaleCosts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Date,Value,CategoryId,LicensePlate,Car")] AfterSaleCost cost)
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

        // GET: AfterSaleCosts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cost = await _context.AfterSaleCosts.FindAsync(id);
            if (cost == null)
            {
                return NotFound();
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", cost.CategoryId);
            return View(cost);
        }

        // POST: AfterSaleCosts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Date,Value,CategoryId,LicensePlate,Car")] AfterSaleCost cost)
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

        // GET: AfterSaleCosts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cost = await _context.AfterSaleCosts
                .Include(a => a.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cost == null)
            {
                return NotFound();
            }

            return View(cost);
        }

        // POST: AfterSaleCosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cost = await _context.AfterSaleCosts.FindAsync(id);
            _context.AfterSaleCosts.Remove(cost);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CostExists(int id)
        {
            return _context.AfterSaleCosts.Any(e => e.Id == id);
        }
    }
}