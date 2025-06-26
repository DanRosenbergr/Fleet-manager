namespace CarApp.Models {
    public class TripLog {
        public int Id { get; set; }

        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int DaysOut { get; set; } 

        public int DistanceKm { get; set; }        

        public string Purpose { get; set; }

        public int CarId { get; set; }
        public Car Car { get; set; }

        public string? UserID { get; set; }
      
    }
}
