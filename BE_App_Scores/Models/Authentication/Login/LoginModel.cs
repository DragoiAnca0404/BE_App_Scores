using System.ComponentModel.DataAnnotations;

namespace BE_App_Scores.Models.Authentication.Login
{
    public class LoginModel
    {
        [Required(ErrorMessage ="UserName is required!")]
        public string? UsernameOrEmail { get; set; }
        [Required(ErrorMessage ="Password is required")]
        public string? Password {  get; set; }
    }
}
