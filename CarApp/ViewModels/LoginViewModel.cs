using System.ComponentModel.DataAnnotations;

namespace CarApp.ViewModels {
    public class LoginViewModel {
        [Required(ErrorMessage = "Enter user name")]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Enter password")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public bool Remember { get; set; }

        public bool DemoUser { get; set; }
    }
}
