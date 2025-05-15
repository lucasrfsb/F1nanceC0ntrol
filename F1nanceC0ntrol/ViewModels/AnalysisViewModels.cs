using F1nanceC0ntrol.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace F1nanceC0ntrol.ViewModels
{
    // ViewModel para análise de todos os custos
    public class AllCostsAnalysisViewModel
    {
        [Display(Name = "Data Inicial")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Display(Name = "Data Final")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Display(Name = "Categoria")]
        public int? CategoryId { get; set; }

        public SelectList Categories { get; set; }

        [Display(Name = "Ordenar Por")]
        public string SortBy { get; set; } // "Date" ou "Value"

        [Display(Name = "Ordem")]
        public string SortOrder { get; set; } // "Asc" ou "Desc"

        public List<CostItem> Costs { get; set; } = new List<CostItem>();

        public decimal TotalValue { get; set; }
    }

    public class CostItem
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; }
        public decimal Value { get; set; }
        public string Description { get; set; }
    }

    // ViewModel para análise de custos fixos
    public class FixedCostsAnalysisViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string GroupBy { get; set; } // "Category", "Month", "Year"
    }

    // ViewModel para análise de custo e lucro por carro
    public class CarProfitCostAnalysisViewModel
    {
        [Display(Name = "Data Inicial")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Display(Name = "Data Final")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Display(Name = "Placa")]
        public string LicensePlate { get; set; }

        [Display(Name = "Carro")]
        public string Car { get; set; }

        [Display(Name = "Mostrar Custos")]
        public bool ShowCosts { get; set; } = true;

        [Display(Name = "Mostrar Vendas")]
        public bool ShowSales { get; set; } = true;

        [Display(Name = "Mostrar Lucros")]
        public bool ShowProfits { get; set; } = true;

        public List<CarTransactionItem> Transactions { get; set; } = new List<CarTransactionItem>();

        public List<CarSummary> CarSummaries { get; set; } = new List<CarSummary>();

        public decimal TotalCosts { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TotalProfits { get; set; }
        public decimal NetResult { get; set; }
    }

    public class CarTransactionItem
    {
        public int Id { get; set; }
        public string Type { get; set; } // "Custo", "Venda", "Lucro"
        public string TransactionType { get; set; } // Tipo específico da transação
        public DateTime Date { get; set; }
        public string LicensePlate { get; set; }
        public string Car { get; set; }
        public decimal Value { get; set; }
        public string Description { get; set; }
    }

    public class CarSummary
    {
        public string LicensePlate { get; set; }
        public string Car { get; set; }
        public decimal TotalCosts { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TotalProfits { get; set; }
        public decimal NetResult { get; set; }
        public int TransactionCount { get; set; }
    }

    // ViewModel para análise de retorno financeiro
    public class FinancialReturnAnalysisViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Bank { get; set; } // Opcional, para filtrar por banco
        public string GroupBy { get; set; } // "Bank", "Month", "Year"
    }

    // ViewModel para análise de todos os custos, vendas e lucros
    public class AllTransactionsAnalysisViewModel
    {
        [Display(Name = "Data Inicial")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Display(Name = "Data Final")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Display(Name = "Categorias")]
        public List<int?> CategoryIds { get; set; } = new List<int?>();

        [Display(Name = "Placa")]
        public string LicensePlate { get; set; }

        [Display(Name = "Carro")]
        public string Car { get; set; }

        [Display(Name = "Mostrar Custos")]
        public bool ShowCosts { get; set; } = true;

        [Display(Name = "Mostrar Vendas")]
        public bool ShowSales { get; set; } = true;

        [Display(Name = "Ordenar Por")]
        public string SortBy { get; set; } // "Date" ou "Value"

        [Display(Name = "Ordem")]
        public string SortOrder { get; set; } // "Asc" ou "Desc"

        public List<Category> AvailableCategories { get; set; } = new List<Category>();

        public List<TransactionItem> Transactions { get; set; } = new List<TransactionItem>();

        public decimal TotalCosts { get; set; }
        public decimal TotalSales { get; set; }
        public decimal ResultadoLiquido => TotalSales - TotalCosts;
    }

    public class TransactionItem
    {
        public int Id { get; set; }
        public string Type { get; set; } // "Custo" ou "Venda"
        public string TransactionType { get; set; } // Tipo específico da transação
        public DateTime Date { get; set; }
        public string Category { get; set; }
        public int? CategoryId { get; set; }
        public string LicensePlate { get; set; }
        public string Car { get; set; }
        public string Bank { get; set; }
        public decimal Value { get; set; }
        public string Description { get; set; }
    }
}