namespace CarApp.DTO {
    public class CarDTO {

        public int Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public int Mileage { get; set; }
        public string Color { get; set; }
        public string LicensePlate { get; set; }
        public FuelType Fuel { get; set; }
        public DateOnly NextMOT { get; set; } // Next MOT date

        public List<RepairDTO> Repairs { get; set; }
        public List<TripLogDTO> TripLogs { get; set; }
    }
}
