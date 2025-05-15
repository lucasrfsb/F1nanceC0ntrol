using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using F1nanceC0ntrol.Data;
using F1nanceC0ntrol.Models;
using F1nanceC0ntrol.ViewModels;
using System.Text.Json;

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

        #region Análise de Lucro e Custo ao Longo do Tempo

        public IActionResult ProfitCost()
        {
            // Obter datas mínima e máxima para os filtros
            var minDate = DateTime.Now.AddYears(-1);
            var maxDate = DateTime.Now;

            try
            {
                var minDateFromDb = _context.FixedCosts.Min(c => c.Date);
                if (minDateFromDb != default)
                    minDate = minDateFromDb;
            }
            catch { /* Ignorar se não houver dados */ }

            var viewModel = new ProfitCostAnalysisViewModel
            {
                StartDate = minDate,
                EndDate = maxDate,
                AggregationType = "Month" // Padrão: agregação por mês
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<JsonResult> GetProfitCostData([FromBody] ProfitCostFilterModel filter)
        {
            // Validar e configurar datas
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
            var costs = new List<object>();

            // 1. FixedCosts
            var fixedCosts = await _context.FixedCosts
                .Where(c => c.Date >= startDate && c.Date <= endDate)
                .Select(c => new { c.Date, c.Value })
                .ToListAsync();
            costs.AddRange(fixedCosts);

            // 2. DailyOperationCosts
            var dailyCosts = await _context.DailyOperationCosts
                .Where(c => c.Date >= startDate && c.Date <= endDate)
                .Select(c => new { c.Date, c.Value })
                .ToListAsync();
            costs.AddRange(dailyCosts);

            // 3. AfterSaleCosts
            var afterSaleCosts = await _context.AfterSaleCosts
                .Where(c => c.Date >= startDate && c.Date <= endDate)
                .Select(c => new { c.Date, c.Value })
                .ToListAsync();
            costs.AddRange(afterSaleCosts);

            // 4. CarCosts
            var carCosts = await _context.CarCosts
                .Where(c => c.Date >= startDate && c.Date <= endDate)
                .Select(c => new { c.Date, c.Value })
                .ToListAsync();
            costs.AddRange(carCosts);

            // 5. SellerCommissions
            var commissions = await _context.SellerCommissions
                .Where(c => c.Date >= startDate && c.Date <= endDate)
                .Select(c => new { c.Date, c.Value })
                .ToListAsync();
            costs.AddRange(commissions);

            // Consultar dados de lucro
            var profits = new List<object>();

            // 1. FinancingReturns
            var financingReturns = await _context.FinancingReturns
                .Where(p => p.Date >= startDate && p.Date <= endDate)
                .Select(p => new { p.Date, p.Value })
                .ToListAsync();
            profits.AddRange(financingReturns);

            // 2. CarSaleProfits (precisamos calcular o lucro)
            var carSales = await _context.CarSaleProfits
                .Where(s => s.Date >= startDate && s.Date <= endDate)
                .ToListAsync();

            foreach (var sale in carSales)
            {
                // Obter todos os custos para este carro
                var totalCarCost = await _context.CarCosts
                    .Where(c => c.LicensePlate == sale.LicensePlate)
                    .SumAsync(c => c.Value);

                var totalAfterSaleCost = await _context.AfterSaleCosts
                    .Where(c => c.LicensePlate == sale.LicensePlate)
                    .SumAsync(c => c.Value);

                // Calcular o lucro como valor de venda menos o total de custos
                var profit = sale.Value - (totalCarCost + totalAfterSaleCost);

                profits.Add(new { sale.Date, Value = profit });
            }

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
                        .Where(c => ((dynamic)c).Date >= monthStart && ((dynamic)c).Date <= monthEnd)
                        .Sum(c => (decimal)((dynamic)c).Value);

                    var monthlyProfits = profits
                        .Where(p => ((dynamic)p).Date >= monthStart && ((dynamic)p).Date <= monthEnd)
                        .Sum(p => (decimal)((dynamic)p).Value);

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
                        .Where(c => ((dynamic)c).Date.Year == year)
                        .Sum(c => (decimal)((dynamic)c).Value);

                    var yearlyProfits = profits
                        .Where(p => ((dynamic)p).Date.Year == year)
                        .Sum(p => (decimal)((dynamic)p).Value);

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

        #endregion

        #region Análise de Todos os Custos

        public async Task<IActionResult> AllCosts(
            DateTime? startDate = null,
            DateTime? endDate = null,
            int? categoryId = null,
            string sortBy = "Date",
            string sortOrder = "Desc")
        {
            // Configurar datas padrão se não forem fornecidas
            var model = new AllCostsAnalysisViewModel
            {
                StartDate = startDate ?? DateTime.Now.AddMonths(-1),
                EndDate = endDate ?? DateTime.Now,
                CategoryId = categoryId,
                SortBy = sortBy ?? "Date",
                SortOrder = sortOrder ?? "Desc"
            };

            // Carregar categorias para o dropdown
            model.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");

            // Ajustar a data final para incluir todo o dia
            var endDateAdjusted = model.EndDate.AddDays(1).AddSeconds(-1);

            // Consultar todos os tipos de custos
            var costs = new List<CostItem>();

            // 1. SellerCommissions
            var commissions = await _context.SellerCommissions
                .Include(c => c.Category)
                .Where(c => c.Date >= model.StartDate && c.Date <= endDateAdjusted)
                .Where(c => model.CategoryId == null || c.CategoryId == model.CategoryId)
                .Select(c => new CostItem
                {
                    Id = c.Id,
                    Type = "Comissionamento",
                    Date = c.Date,
                    Category = c.Category.Name,
                    Value = c.Value,
                    Description = $"Funcionário: {c.EmployeeName}"
                })
                .ToListAsync();
            costs.AddRange(commissions);

            // 2. DailyOperationCosts
            var dailyCosts = await _context.DailyOperationCosts
                .Include(c => c.Category)
                .Where(c => c.Date >= model.StartDate && c.Date <= endDateAdjusted)
                .Where(c => model.CategoryId == null || c.CategoryId == model.CategoryId)
                .Select(c => new CostItem
                {
                    Id = c.Id,
                    Type = "Custo Diário",
                    Date = c.Date,
                    Category = c.Category.Name,
                    Value = c.Value,
                    Description = "Custo de operação diária"
                })
                .ToListAsync();
            costs.AddRange(dailyCosts);

            // 3. FixedCosts
            var fixedCosts = await _context.FixedCosts
                .Include(c => c.Category)
                .Where(c => c.Date >= model.StartDate && c.Date <= endDateAdjusted)
                .Where(c => model.CategoryId == null || c.CategoryId == model.CategoryId)
                .Select(c => new CostItem
                {
                    Id = c.Id,
                    Type = "Custo Fixo",
                    Date = c.Date,
                    Category = c.Category.Name,
                    Value = c.Value,
                    Description = "Custo fixo mensal"
                })
                .ToListAsync();
            costs.AddRange(fixedCosts);

            // 4. AfterSaleCosts
            var afterSaleCosts = await _context.AfterSaleCosts
                .Include(c => c.Category)
                .Where(c => c.Date >= model.StartDate && c.Date <= endDateAdjusted)
                .Where(c => model.CategoryId == null || c.CategoryId == model.CategoryId)
                .Select(c => new CostItem
                {
                    Id = c.Id,
                    Type = "Pós-Venda",
                    Date = c.Date,
                    Category = c.Category.Name,
                    Value = c.Value,
                    Description = $"Carro: {c.Car}, Placa: {c.LicensePlate}"
                })
                .ToListAsync();
            costs.AddRange(afterSaleCosts);

            // 5. CarCosts
            var carCosts = await _context.CarCosts
                .Include(c => c.Category)
                .Where(c => c.Date >= model.StartDate && c.Date <= endDateAdjusted)
                .Where(c => model.CategoryId == null || c.CategoryId == model.CategoryId)
                .Select(c => new CostItem
                {
                    Id = c.Id,
                    Type = "Custo de Carro",
                    Date = c.Date,
                    Category = c.Category.Name,
                    Value = c.Value,
                    Description = $"Carro: {c.Car}, Placa: {c.LicensePlate}"
                })
                .ToListAsync();
            costs.AddRange(carCosts);

            // Ordenar os resultados
            if (model.SortBy == "Date")
            {
                costs = model.SortOrder == "Asc"
                    ? costs.OrderBy(c => c.Date).ToList()
                    : costs.OrderByDescending(c => c.Date).ToList();
            }
            else // Value
            {
                costs = model.SortOrder == "Asc"
                    ? costs.OrderBy(c => c.Value).ToList()
                    : costs.OrderByDescending(c => c.Value).ToList();
            }

            // Calcular o total
            model.TotalValue = costs.Sum(c => c.Value);

            // Atribuir os custos ao modelo
            model.Costs = costs;

            return View(model);
        }

        #endregion

        #region Análise de Custos, Vendas e Lucros de cada Carro

        public async Task<IActionResult> CarProfitCost(
            DateTime? startDate = null,
            DateTime? endDate = null,
            string licensePlate = null,
            string car = null,
            bool? showCosts = true,
            bool? showSales = true,
            bool? showProfits = true)
        {
            // Configurar datas padrão se não forem fornecidas
            var model = new CarProfitCostAnalysisViewModel
            {
                StartDate = startDate ?? DateTime.Now.AddMonths(-3),
                EndDate = endDate ?? DateTime.Now,
                LicensePlate = licensePlate,
                Car = car,
                ShowCosts = showCosts ?? true,
                ShowSales = showSales ?? true,
                ShowProfits = showProfits ?? true
            };

            // Ajustar a data final para incluir todo o dia
            var endDateAdjusted = model.EndDate.AddDays(1).AddSeconds(-1);

            // Lista para armazenar todas as transações
            var transactions = new List<CarTransactionItem>();

            // Primeiro, precisamos obter todos os custos e vendas para calcular o lucro corretamente

            // 1. Custos de Carro
            var carCosts = await _context.CarCosts
                .Include(c => c.Category)
                .Where(c => c.Date >= model.StartDate && c.Date <= endDateAdjusted)
                .Where(c => string.IsNullOrEmpty(model.LicensePlate) || c.LicensePlate.Contains(model.LicensePlate))
                .Where(c => string.IsNullOrEmpty(model.Car) || c.Car.Contains(model.Car))
                .ToListAsync();

            // 2. Custos de pós-venda
            var afterSaleCosts = await _context.AfterSaleCosts
                .Include(c => c.Category)
                .Where(c => c.Date >= model.StartDate && c.Date <= endDateAdjusted)
                .Where(c => string.IsNullOrEmpty(model.LicensePlate) || c.LicensePlate.Contains(model.LicensePlate))
                .Where(c => string.IsNullOrEmpty(model.Car) || c.Car.Contains(model.Car))
                .ToListAsync();

            // 3. Vendas de Carro
            var carSales = await _context.CarSaleProfits
                .Include(c => c.Category)
                .Where(c => c.Date >= model.StartDate && c.Date <= endDateAdjusted)
                .Where(c => string.IsNullOrEmpty(model.LicensePlate) || c.LicensePlate.Contains(model.LicensePlate))
                .Where(c => string.IsNullOrEmpty(model.Car) || c.Car.Contains(model.Car))
                .ToListAsync();

            // Adicionar custos à lista de transações (se selecionado)
            if (model.ShowCosts)
            {
                // Custos de compra de carros
                var carCostTransactions = carCosts.Select(c => new CarTransactionItem
                {
                    Id = c.Id,
                    Type = "Custo",
                    TransactionType = "Compra/Manutenção",
                    Date = c.Date,
                    LicensePlate = c.LicensePlate,
                    Car = c.Car,
                    Value = c.Value,
                    Description = $"Custo - {c.Category.Name}"
                }).ToList();
                transactions.AddRange(carCostTransactions);

                // Custos de pós-venda
                var afterSaleCostTransactions = afterSaleCosts.Select(c => new CarTransactionItem
                {
                    Id = c.Id,
                    Type = "Custo",
                    TransactionType = "Pós-Venda",
                    Date = c.Date,
                    LicensePlate = c.LicensePlate,
                    Car = c.Car,
                    Value = c.Value,
                    Description = $"Pós-Venda - {c.Category.Name}"
                }).ToList();
                transactions.AddRange(afterSaleCostTransactions);
            }

            // Adicionar vendas à lista de transações (se selecionado)
            if (model.ShowSales)
            {
                var saleTransactions = carSales.Select(c => new CarTransactionItem
                {
                    Id = c.Id,
                    Type = "Venda",
                    TransactionType = "Venda de Carro",
                    Date = c.Date,
                    LicensePlate = c.LicensePlate,
                    Car = c.Car,
                    Value = c.Value, // Valor da venda
                    Description = $"Venda - {c.Category.Name}"
                }).ToList();
                transactions.AddRange(saleTransactions);
            }

            // Adicionar lucros à lista de transações (se selecionado)
            if (model.ShowProfits)
            {
                // Para cada venda, calcular o lucro somando todos os custos associados ao carro
                foreach (var sale in carSales)
                {
                    // Somar todos os custos para este carro específico
                    var costForCar = carCosts
                        .Where(c => c.LicensePlate == sale.LicensePlate)
                        .Sum(c => c.Value);

                    costForCar += afterSaleCosts
                        .Where(c => c.LicensePlate == sale.LicensePlate)
                        .Sum(c => c.Value);

                    // Calcular o lucro como valor de venda menos o total de custos
                    var profit = sale.Value - costForCar;

                    transactions.Add(new CarTransactionItem
                    {
                        Id = sale.Id,
                        Type = "Lucro",
                        TransactionType = "Lucro de Venda",
                        Date = sale.Date,
                        LicensePlate = sale.LicensePlate,
                        Car = sale.Car,
                        Value = profit,
                        Description = $"Lucro (Venda - Custos) - {sale.Category.Name}"
                    });
                }
            }

            // Ordenar as transações por data (mais recente primeiro)
            transactions = transactions.OrderByDescending(t => t.Date).ToList();

            // Calcular totais
            model.TotalCosts = transactions.Where(t => t.Type == "Custo").Sum(t => t.Value);
            model.TotalSales = transactions.Where(t => t.Type == "Venda").Sum(t => t.Value);
            model.TotalProfits = transactions.Where(t => t.Type == "Lucro").Sum(t => t.Value);
            model.NetResult = model.TotalSales - model.TotalCosts;

            // Agrupar por carro para estatísticas
            var carSummaries = transactions
                .GroupBy(t => new { t.LicensePlate, t.Car })
                .Select(g => new CarSummary
                {
                    LicensePlate = g.Key.LicensePlate,
                    Car = g.Key.Car,
                    TotalCosts = g.Where(t => t.Type == "Custo").Sum(t => t.Value),
                    TotalSales = g.Where(t => t.Type == "Venda").Sum(t => t.Value),
                    TotalProfits = g.Where(t => t.Type == "Lucro").Sum(t => t.Value),
                    NetResult = g.Where(t => t.Type == "Venda").Sum(t => t.Value) - g.Where(t => t.Type == "Custo").Sum(t => t.Value),
                    TransactionCount = g.Count()
                })
                .OrderByDescending(s => s.NetResult)
                .ToList();

            model.CarSummaries = carSummaries;

            // Atribuir as transações ao modelo
            model.Transactions = transactions;

            return View(model);
        }

        #endregion

        #region Análise de Custos Fixos

        public IActionResult FixedCosts()
        {
            // Implementação futura
            return View();
        }

        #endregion

        #region Análise de Retorno Financeiro

        public IActionResult FinancialReturn()
        {
            // Implementação futura
            return View();
        }

        #endregion
    }
}