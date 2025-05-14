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
    public class FinancingReturnsController : Controller
    {
        private readonly FinancialDbContext _context;

        public FinancingReturnsController(FinancialDbContext context)
        {
            _context = context;
        }

        // GET: FinancingReturns
        public async Task<IActionResult> Index()
        {
            var returns = await _context.FinancingReturns
                .Include(f => f.Category)
                .ToListAsync();
            return View(returns);
        }

        // GET: FinancingReturns/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var financingReturn = await _context.FinancingReturns
                .Include(f => f.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (financingReturn == null)
            {
                return NotFound();
            }

            return View(financingReturn);
        }

        // GET: FinancingReturns/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: FinancingReturns/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Date,Value,CategoryId,Bank")] FinancingReturn financingReturn)
        {
            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                _context.Add(financingReturn);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", financingReturn.CategoryId);
            return View(financingReturn);
        }

        // GET: FinancingReturns/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var financingReturn = await _context.FinancingReturns.FindAsync(id);
            if (financingReturn == null)
            {
                return NotFound();
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", financingReturn.CategoryId);
            return View(financingReturn);
        }

        // POST: FinancingReturns/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Date,Value,CategoryId,Bank")] FinancingReturn financingReturn)
        {
            if (id != financingReturn.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(financingReturn);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FinancingReturnExists(financingReturn.Id))
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

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", financingReturn.CategoryId);
            return View(financingReturn);
        }

        // GET: FinancingReturns/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var financingReturn = await _context.FinancingReturns
                .Include(f => f.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (financingReturn == null)
            {
                return NotFound();
            }

            return View(financingReturn);
        }

        // POST: FinancingReturns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var financingReturn = await _context.FinancingReturns.FindAsync(id);
            _context.FinancingReturns.Remove(financingReturn);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FinancingReturnExists(int id)
        {
            return _context.FinancingReturns.Any(e => e.Id == id);
        }
    }
}