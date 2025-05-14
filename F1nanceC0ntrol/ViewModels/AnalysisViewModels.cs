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
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string LicensePlate { get; set; } // Opcional, para filtrar por placa
        public string Car { get; set; } // Opcional, para filtrar por modelo
    }

    // ViewModel para análise de retorno financeiro
    public class FinancialReturnAnalysisViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Bank { get; set; } // Opcional, para filtrar por banco
        public string GroupBy { get; set; } // "Bank", "Month", "Year"
    }
}