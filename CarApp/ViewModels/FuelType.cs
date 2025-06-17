using System.ComponentModel.DataAnnotations;

public enum FuelType {
    [Display(Name = "Petrol")]
    Petrol = 0,

    [Display(Name = "Diesel")]
    Diesel = 1,

    [Display(Name = "LPG")]
    LPG = 2,

    [Display(Name = "Electric")]
    Electric = 3,

    [Display(Name = "Hybrid")]
    Hybrid = 4
}
