using CarApp.DTO;

namespace CarApp.ViewModels {
    public class RepairStatsViewModel {

        public List<RepairDTO> Repairs { get; set; }

        // Fleet Statistiky
        public int TotalRepairs { get; set; }
        public decimal TotalCost { get; set; }
        public decimal AverageCost { get; set; }
        public int DaysOff { get; set; }
        //car statistics
        public decimal HighCost { get; set; }
        public string? HighCostCarBrand { get; set; }
        public string? HighCostCarModel { get; set; }

        public int LongestOff { get; set; }
        public string? LongestOffCarBrand { get; set; }
        public string? LongestOffCarModel { get; set; }


        public int MostRepairedCount { get; set; }
        public string? MostRepairedCarBrand { get; set; }
        public string? MostRepairedCarModel { get; set; }
    }
}
