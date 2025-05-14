using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using F1nanceC0ntrol.Data;
using F1nanceC0ntrol.Models;
using F1nanceC0ntrol.ViewModels;

namespace F1nanceC0ntrol.Controllers
{
    public class AnalysisController : Controller
    {
        private readonly FinancialDbContext _context;

        public AnalysisController(FinancialDbContext context)
        {
            _context = context;
        }

        // Página principal de análise com os 5 botões
        public IActionResult Index()
        {
            return View();
        }

        // Métodos para cada tipo de análise
        public IActionResult ProfitCost()
        {
            // Obter datas mínima e máxima para os filtros
            var minDate = _context.FixedCosts.Min(c => c.Date);
            var maxDate = _context.FixedCosts.Max(c => c.Date);

            if (minDate == DateTime.MinValue)
                minDate = DateTime.Now.AddYears(-1);
            if (maxDate == DateTime.MinValue)
                maxDate = DateTime.Now;

            var viewModel = new ProfitCostAnalysisViewModel
            {
                StartDate = minDate,
                EndDate = maxDate,
                AggregationType = "Month" // Padrão: agregação por mês
            };

            return View(viewModel);
        }

        public IActionResult AllCosts()
        {
            // Implementação futura
            return View();
        }

        public IActionResult FixedCosts()
        {
            // Implementação futura
            return View();
        }

        public IActionResult CarProfitCost()
        {
            // Implementação futura
            return View();
        }

        public IActionResult FinancialReturn()
        {
            // Implementação futura
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetProfitCostData([FromBody] ProfitCostFilterModel filter)
        {
            DateTime startDate = filter.StartDate;
            DateTime endDate = filter.EndDate;
            string aggregationType = filter.AggregationType;

            // Validar datas
            if (startDate == DateTime.MinValue)
                startDate = DateTime.Now.AddYears(-1);
            if (endDate == DateTime.MinValue)
                endDate = DateTime.Now;
            if (startDate > endDate)
            {
                var temp = startDate;
                startDate = endDate;
                endDate = temp;
            }
            if (string.IsNullOrEmpty(aggregationType))
                aggregationType = "Month";

            // Ajustar endDate para incluir todo o dia
            endDate = endDate.AddDays(1).AddSeconds(-1);

            // Consultar dados de custo
            var costs = await (
                from fc in _context.FixedCosts
                where fc.Date >= startDate && fc.Date <= endDate
                select new { fc.Date, fc.Value }
            ).ToListAsync();

            costs = costs.Concat(
                await (
                    from dc in _context.DailyOperationCosts
                    where dc.Date >= startDate && dc.Date <= endDate
                    select new { dc.Date, dc.Value }
                ).ToListAsync()
            ).ToList();

            costs = costs.Concat(
                await (
                    from ac in _context.AfterSaleCosts
                    where ac.Date >= startDate && ac.Date <= endDate
                    select new { ac.Date, ac.Value }
                ).ToListAsync()
            ).ToList();

            costs = costs.Concat(
                await (
                    from cc in _context.CarCosts
                    where cc.Date >= startDate && cc.Date <= endDate
                    select new { cc.Date, cc.Value }
                ).ToListAsync()
            ).ToList();

            costs = costs.Concat(
                await (
                    from sc in _context.SellerCommissions
                    where sc.Date >= startDate && sc.Date <= endDate
                    select new { sc.Date, sc.Value }
                ).ToListAsync()
            ).ToList();

            // Consultar dados de lucro (apenas FinancingReturns neste caso)
            var profits = await (
                from fr in _context.FinancingReturns
                where fr.Date >= startDate && fr.Date <= endDate
                select new { fr.Date, fr.Value }
            ).ToListAsync();

            // Agregar dados por mês ou ano
            var costData = new List<decimal>();
            var profitData = new List<decimal>();
            var labels = new List<string>();

            if (aggregationType == "Month")
            {
                // Agrupar por mês
                var months = Enumerable.Range(0, (endDate.Year - startDate.Year) * 12 + endDate.Month - startDate.Month + 1)
                    .Select(m => startDate.AddMonths(m))
                    .ToList();

                foreach (var month in months)
                {
                    var monthStart = new DateTime(month.Year, month.Month, 1);
                    var monthEnd = monthStart.AddMonths(1).AddDays(-1);

                    var monthlyCosts = costs
                        .Where(c => c.Date >= monthStart && c.Date <= monthEnd)
                        .Sum(c => c.Value);

                    var monthlyProfits = profits
                        .Where(p => p.Date >= monthStart && p.Date <= monthEnd)
                        .Sum(p => p.Value);

                    costData.Add(monthlyCosts);
                    profitData.Add(monthlyProfits);
                    labels.Add(month.ToString("MMM yyyy"));
                }
            }
            else // Year
            {
                // Agrupar por ano
                var years = Enumerable.Range(startDate.Year, endDate.Year - startDate.Year + 1).ToList();

                foreach (var year in years)
                {
                    var yearStart = new DateTime(year, 1, 1);
                    var yearEnd = new DateTime(year, 12, 31);

                    var yearlyCosts = costs
                        .Where(c => c.Date.Year == year)
                        .Sum(c => c.Value);

                    var yearlyProfits = profits
                        .Where(p => p.Date.Year == year)
                        .Sum(p => p.Value);

                    costData.Add(yearlyCosts);
                    profitData.Add(yearlyProfits);
                    labels.Add(year.ToString());
                }
            }

            // Calcular soma do lucro (linha verde no gráfico)
            var profitSum = new List<decimal>();
            decimal runningSum = 0;

            foreach (var profit in profitData)
            {
                runningSum += profit;
                profitSum.Add(runningSum);
            }

            return Json(new
            {
                labels,
                datasets = new object[]
                {
                    new
                    {
                        label = "Custo",
                        data = costData,
                        backgroundColor = "rgba(255, 99, 132, 0.7)",
                        borderColor = "rgba(255, 99, 132, 1)",
                        borderWidth = 1,
                        type = "bar"
                    },
                    new
                    {
                        label = "Lucro",
                        data = profitData,
                        backgroundColor = "rgba(54, 162, 235, 0.7)",
                        borderColor = "rgba(54, 162, 235, 1)",
                        borderWidth = 1,
                        type = "bar"
                    },
                    new
                    {
                        label = "Soma do Lucro",
                        data = profitSum,
                        borderColor = "rgba(75, 192, 192, 1)",
                        backgroundColor = "rgba(0, 0, 0, 0)",
                        borderWidth = 2,
                        type = "line",
                        tension = 0.1
                    }
                }
            });
        }
    }
}