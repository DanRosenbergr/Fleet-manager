using System.ComponentModel.DataAnnotations;

namespace CarApp.ViewModels {
    public class RegisterViewModel {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

       
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password again!")]
        [Compare("Password", ErrorMessage = "Password does not match!")]
        public string ConfirmPassword { get; set; }
    }
}
