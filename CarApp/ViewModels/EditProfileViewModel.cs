using System.ComponentModel.DataAnnotations;

namespace CarApp.ViewModels {
    public class EditProfileViewModel {

        [Required]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "New password and confirmation do not match.")]
        public string? ConfirmNewPassword { get; set; }
    }
}
