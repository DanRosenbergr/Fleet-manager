namespace CarApp.Models {
    public class Car {

        public int Id { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public int Year { get; set; }
        
        public string Color { get; set; }

        public string LicensePlate { get; set; }

        public int Mileage { get; set; }

        public FuelType Fuel { get; set; }

        public DateOnly NextMOT { get; set; } // Next MOT date

        public ICollection<TripLog> TripLogs { get; set; } = new List<TripLog>();

        public string UserID { get; set; }
      

    }
}
