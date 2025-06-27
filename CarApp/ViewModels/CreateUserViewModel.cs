using System.ComponentModel.DataAnnotations;

namespace CarApp.ViewModels {
    public class CreateUserViewModel {
        [Required]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
