using System.ComponentModel.DataAnnotations;

namespace CarApp.ViewModels {
    public class EditUserViewModel {

        public string Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required.")]
        public string NewPassword { get; set; }
    }
}
