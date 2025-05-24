using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using F1nanceC0ntrol.Data;
using F1nanceC0ntrol.Models;

namespace F1nanceC0ntrol.Controllers
{
    public class CarSaleProfitsController : Controller
    {
        private readonly FinancialDbContext _context;

        public CarSaleProfitsController(FinancialDbContext context)
        {
            _context = context;
        }

        // GET: CarSaleProfits
        public async Task<IActionResult> Index()
        {
            var financialDbContext = _context.CarSaleProfits.Include(c => c.Category);
            return View(await financialDbContext.ToListAsync());
        }

        // GET: CarSaleProfits/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carSaleProfit = await _context.CarSaleProfits
                .Include(c => c.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (carSaleProfit == null)
            {
                return NotFound();
            }

            return View(carSaleProfit);
        }

        // GET: CarSaleProfits/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = _context.Categories
            .Where(c => c.Name == "Venda de Carros")
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name

            })
            .ToList();
            var model = new CarSaleProfit { Date = DateTime.Now };
            return View(model);
        }

        // POST: CarSaleProfits/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Date,CategoryId,LicensePlate,Car,Value")] CarSaleProfit carSaleProfit)
        {
            ModelState.Remove("Category");
            if (ModelState.IsValid)
            {
                _context.Add(carSaleProfit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = _context.Categories
            .Where(c => c.Name == "Venda de Carros")
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name

            })
            .ToList();
            return View(carSaleProfit);
        }

        // GET: CarSaleProfits/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carSaleProfit = await _context.CarSaleProfits.FindAsync(id);
            if (carSaleProfit == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", carSaleProfit.CategoryId);
            return View(carSaleProfit);
        }

        // POST: CarSaleProfits/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Date,CategoryId,LicensePlate,Car,Value")] CarSaleProfit carSaleProfit)
        {
            if (id != carSaleProfit.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carSaleProfit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarSaleProfitExists(carSaleProfit.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", carSaleProfit.CategoryId);
            return View(carSaleProfit);
        }

        // GET: CarSaleProfits/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carSaleProfit = await _context.CarSaleProfits
                .Include(c => c.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (carSaleProfit == null)
            {
                return NotFound();
            }

            return View(carSaleProfit);
        }

        // POST: CarSaleProfits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var carSaleProfit = await _context.CarSaleProfits.FindAsync(id);
            _context.CarSaleProfits.Remove(carSaleProfit);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarSaleProfitExists(int id)
        {
            return _context.CarSaleProfits.Any(e => e.Id == id);
        }
    }
}