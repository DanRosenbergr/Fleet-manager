using CarApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CarApp.DTO {
    public class RepairDTO {
        public int Id { get; set; }

        public string Description { get; set; }

        public DateOnly RepairDateStart { get; set; }

        public DateOnly? RepairDateEnd { get; set; }

        public int DaysInService => (RepairDateEnd.HasValue ? RepairDateEnd.Value :
            DateOnly.FromDateTime(DateTime.Today)).DayNumber - RepairDateStart.DayNumber;

        public int MileageAtRepair { get; set; }

        [Precision(10, 2)]
        public decimal Cost { get; set; }

        public bool IsMOT { get; set; }

        public int CarId { get; set; }

        public string? CarBrand { get; set; }
        public string? CarModel { get; set; }

    }
}
