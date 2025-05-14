using System;
using System.Collections.Generic;

namespace F1nanceC0ntrol.ViewModels
{
    // ViewModel para análise de todos os custos
    public class AllCostsAnalysisViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string GroupBy { get; set; } // "Category", "Month", "Year"
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