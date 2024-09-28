using Microsoft.AspNetCore.Identity;

namespace AuthApp.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}
