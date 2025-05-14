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
    public class CarCostsController : Controller
    {
        private readonly FinancialDbContext _context;

        public CarCostsController(FinancialDbContext context)
        {
            _context = context;
        }

        // GET: CarCosts
        public async Task<IActionResult> Index()
        {
            var costs = await _context.CarCosts
                .Include(c => c.Category)
                .ToListAsync();
            return View(costs);
        }

        // GET: CarCosts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cost = await _context.CarCosts
                .Include(c => c.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cost == null)
            {
                return NotFound();
            }

            return View(cost);
        }

        // GET: CarCosts/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: CarCosts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Date,Value,CategoryId,LicensePlate,Car")] CarCost cost)
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

        // GET: CarCosts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cost = await _context.CarCosts.FindAsync(id);
            if (cost == null)
            {
                return NotFound();
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", cost.CategoryId);
            return View(cost);
        }

        // POST: CarCosts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Date,Value,CategoryId,LicensePlate,Car")] CarCost cost)
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

        // GET: CarCosts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cost = await _context.CarCosts
                .Include(c => c.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cost == null)
            {
                return NotFound();
            }

            return View(cost);
        }

        // POST: CarCosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cost = await _context.CarCosts.FindAsync(id);
            _context.CarCosts.Remove(cost);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CostExists(int id)
        {
            return _context.CarCosts.Any(e => e.Id == id);
        }
    }
}