using Microsoft.AspNetCore.Identity;
namespace ProgramGuard.Models
{
    public class AppUser : IdentityUser
    {
        public bool IsFrozen { get; set; }
        public DateTime LastPasswordChangedDate { get; set; }
        public virtual ICollection<LoginHistory> LoginHistories { get; set; }
        //public virtual ICollection<IdentityUserRole<string>> Roles { get; } = new List<IdentityUserRole<string>>();
    }
}
