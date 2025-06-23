using CarApp.DTO;

namespace CarApp.ViewModels {
    public class TripLogStatsViewModel {

        public List<TripLogDTO> TripLogs { get; set; }
        // Fleet Statistics
        public int SumTrips { get; set; }
        public int TotalDistance { get; set; }
        public double AvdDaysOut { get; set; }
        public double AvgDistance { get; set; }

        //car statistics
        public int FurtherstTrip { get; set; }
        public string? FurtherstTripCarBrand { get; set; }
        public string? FurtherstTripCarModel { get; set; }

        public int LongestTrip { get; set; }
        public string? LongestTripCarBrand { get; set; }
        public string? LongestTripCarModel { get; set; }

        public int MostTripsCount { get; set; }
        public string? MostTripsCarBrand { get; set; }
        public string? MostTripsCarModel { get; set; }
    }
}
