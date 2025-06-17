using System.ComponentModel.DataAnnotations;

namespace CarApp.ViewModels {
    public class LoginViewModel {
        [Required(ErrorMessage = "Zadejte uživatelské jméno")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Zadejte heslo")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string? ReturnUrl { get; set; }

        public bool Remember { get; set; }
    }
}
