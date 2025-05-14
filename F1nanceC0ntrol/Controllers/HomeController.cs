using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using F1nanceC0ntrol.Data;
using F1nanceC0ntrol.Models;

namespace F1nanceC0ntrol.Controllers
{
    public class HomeController : Controller
    {
        private readonly FinancialDbContext _context;

        public HomeController(FinancialDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddTransaction()
        {
            return View();
        }

        public IActionResult ViewTransactions()
        {
            return View();
        }

        public IActionResult Analyze()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetTransactionTypeData()
        {
            var sellerCommissions = await _context.SellerCommissions.SumAsync(t => t.Value);
            var dailyOperationCosts = await _context.DailyOperationCosts.SumAsync(t => t.Value);
            var fixedCosts = await _context.FixedCosts.SumAsync(t => t.Value);
            var afterSaleCosts = await _context.AfterSaleCosts.SumAsync(t => t.Value);
            var carCosts = await _context.CarCosts.SumAsync(t => t.Value);
            var financingReturns = await _context.FinancingReturns.SumAsync(t => t.Value);

            var data = new List<decimal>
            {
                sellerCommissions,
                dailyOperationCosts,
                fixedCosts,
                afterSaleCosts,
                carCosts,
                financingReturns
            };

            var labels = new List<string>
            {
                "Comissionamento",
                "Custo Diário",
                "Custos Fixos",
                "Pós-Venda",
                "Custo dos Carros",
                "Financiamento"
            };

            return Json(new { labels, data });
        }

        [HttpGet]
        public async Task<JsonResult> GetMonthlySummary()
        {
            var currentYear = DateTime.Now.Year;
            var months = Enumerable.Range(1, 12).Select(m => new DateTime(currentYear, m, 1)).ToList();

            var incomeData = new List<decimal>();
            var expenseData = new List<decimal>();

            foreach (var month in months)
            {
                var nextMonth = month.AddMonths(1);

                // Receitas (exemplo: retornos de financiamento)
                var income = await _context.FinancingReturns
                    .Where(t => t.Date >= month && t.Date < nextMonth)
                    .SumAsync(t => t.Value);

                // Despesas (soma de vários tipos)
                var expenses = await _context.SellerCommissions
                    .Where(t => t.Date >= month && t.Date < nextMonth)
                    .SumAsync(t => t.Value);
                expenses += await _context.DailyOperationCosts
                    .Where(t => t.Date >= month && t.Date < nextMonth)
                    .SumAsync(t => t.Value);
                expenses += await _context.FixedCosts
                    .Where(t => t.Date >= month && t.Date < nextMonth)
                    .SumAsync(t => t.Value);
                expenses += await _context.AfterSaleCosts
                    .Where(t => t.Date >= month && t.Date < nextMonth)
                    .SumAsync(t => t.Value);
                expenses += await _context.CarCosts
                    .Where(t => t.Date >= month && t.Date < nextMonth)
                    .SumAsync(t => t.Value);

                incomeData.Add(income);
                expenseData.Add(expenses);
            }

            var labels = months.Select(m => m.ToString("MMM")).ToList();

            return Json(new
            {
                labels,
                datasets = new[] {
                    new { label = "Receitas", data = incomeData },
                    new { label = "Despesas", data = expenseData }
                }
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}