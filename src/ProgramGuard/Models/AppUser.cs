using Microsoft.AspNetCore.Identity;
namespace ProgramGuard.Models
{
    public class AppUser : IdentityUser
    {
        public bool IsFrozen { get; set; }
        public DateTime LastPasswordChangedDate { get; set; } = DateTime.UtcNow.ToLocalTime();
        public virtual ICollection<LoginHistory> LoginHistories { get; set; }
        //public int Permissions {  get; set; }
    }
}
