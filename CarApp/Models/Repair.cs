using Microsoft.EntityFrameworkCore;

namespace CarApp.Models {

    public class Repair {
        public int Id { get; set; }

        public string Description { get; set; }

        public DateOnly RepairDateStart { get; set; }
        public DateOnly RepairDateEnd { get; set; }
        public int DaysInService { get; set; }

        public int MileageAtRepair { get; set; } 

        [Precision(10, 2)]
        public decimal Cost { get; set; }       

        public int CarId { get; set; } 
        public Car Car { get; set; }

        public string UserID { get; set; }


    }
}
