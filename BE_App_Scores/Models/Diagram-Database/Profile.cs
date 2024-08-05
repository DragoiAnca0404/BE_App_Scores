using Microsoft.AspNetCore.Identity;

namespace BE_App_Scores.Models.Diagram_Database
{
    public class Profile
    {
        public int Id { get; set; }
        public string UserId { get; set; } // Cheie străină către IdentityUser
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // Navigație
        public IdentityUser User { get; set; }
    }

}
