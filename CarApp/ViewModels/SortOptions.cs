using System.ComponentModel.DataAnnotations;

public enum SortOptions {
    [Display(Name = "Brand")]
    Brand = 1,
    [Display(Name = "Year")]
    Year=2,
    [Display(Name = "Mileage")]
    Mileage=3,
    [Display(Name = "Next MOT")]
    NextMOT=4

}