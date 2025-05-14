using System;
using System.Text.Json.Serialization;

namespace F1nanceC0ntrol.ViewModels
{
    public class ProfitCostAnalysisViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string AggregationType { get; set; } // "Month" ou "Year"
    }

    public class ProfitCostFilterModel
    {
        [JsonPropertyName("startDate")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("endDate")]
        public DateTime EndDate { get; set; }

        [JsonPropertyName("aggregationType")]
        public string AggregationType { get; set; }
    }
}