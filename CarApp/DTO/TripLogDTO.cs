using System.ComponentModel.DataAnnotations;
using CarApp.Models;

namespace CarApp.DTO {
    public class TripLogDTO : IValidatableObject {
        public int Id { get; set; }

        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int DaysOut { get; set; }

        public int DistanceKm { get; set; }

        public string Purpose { get; set; }

        public int CarId { get; set; }

        public string? CarBrand { get; set; }
        public string? CarModel { get; set; }
        public string UserID { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (EndDate < StartDate) {
                yield return new ValidationResult(
                    "Trip end date cannot be earlier than the start date.",
                    new[] { nameof(EndDate) });
            }
        }
    }

}

