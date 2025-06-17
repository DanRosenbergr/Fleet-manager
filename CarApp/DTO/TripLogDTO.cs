using CarApp.Models;

namespace CarApp.DTO {
    public class TripLogDTO {
        public int Id { get; set; }

        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }

        public int DistanceKm { get; set; }

        public string Purpose { get; set; }

        public int CarId { get; set; }

        public string? CarBrand { get; set; }
        public string? CarModel { get; set; }
        public string UserID { get; set; }  

    }

}

