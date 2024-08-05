using System.ComponentModel.DataAnnotations;

namespace BE_App_Scores.Utils
{
    public class TwoFactorLoginModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Code { get; set; }
    }

}
