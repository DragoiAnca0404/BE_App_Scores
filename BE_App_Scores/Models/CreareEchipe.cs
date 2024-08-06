using Microsoft.AspNetCore.Identity;

namespace BE_App_Scores.Models
{
    public class CreareEchipe
    {
        public string UserId { get; set; }
        public IdentityUser User { get; set; }

        public int EchipeId { get; set; }
        public Echipe Echipe { get; set; }
    }
}
