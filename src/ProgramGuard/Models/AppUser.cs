using Microsoft.AspNetCore.Identity;
namespace ProgramGuard.Models
{
    public class AppUser : IdentityUser
    {
        public bool IsFrozen { get; set; }
        public DateTime LastPasswordChangedDate { get; set; } = DateTime.UtcNow.ToLocalTime();
        public DateTime? LastLoginTime { get; set; }
    }
}
