using System.ComponentModel.DataAnnotations;

namespace BE_App_Scores.Models.Authentication.SignUp
{
    public class RegisterUser
    {
        [Required(ErrorMessage = "UserName is required")]

        public string? Username { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "E-mail is required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }

    }
}
