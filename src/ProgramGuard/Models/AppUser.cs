using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace ProgramGuard.Models
{
    public class AppUser : IdentityUser
    {
        public bool IsEnabled { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? LastLoginTime { get; set; }
        [Required]
        public DateTime LastPasswordChangedDate { get; set; } = DateTime.UtcNow.ToLocalTime();

    }
}
