using CarApp.DTO;

namespace CarApp.ViewModels {
    public class TripLogsViewModel {

        public int CarId { get; set; }

        public List<TripLogDTO> TripLogs { get; set; }
    }
}
