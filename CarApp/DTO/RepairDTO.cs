using System.ComponentModel.DataAnnotations;
using CarApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CarApp.DTO {
    public class RepairDTO : IValidatableObject{
        public int Id { get; set; }

        public string Description { get; set; }

        public DateOnly RepairDateStart { get; set; }

        public DateOnly? RepairDateEnd { get; set; }



        public int DaysInService => (RepairDateEnd.HasValue ? RepairDateEnd.Value :
            DateOnly.FromDateTime(DateTime.Today)).DayNumber - RepairDateStart.DayNumber;

        public int MileageAtRepair { get; set; }

        [Precision(10, 2)]
        public decimal Cost { get; set; }

        public int CarId { get; set; }

        public string? CarBrand { get; set; }
        public string? CarModel { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (RepairDateEnd.HasValue && RepairDateEnd < RepairDateStart) {
                yield return new ValidationResult(
                    "Repair end date cannot be earlier than the start date.",
                    new[] { nameof(RepairDateEnd) });
            }
        }
    }
}
