using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ProgramGuard.Models
{
    public class AppUser : IdentityUser
    {
        public bool IsDisabled { get; set; }
        public DateTime? LastLoginTime { get; set; }
        [Required]
        public DateTime LastPasswordChangedDate { get; set; } = DateTime.UtcNow.ToLocalTime();

    }
}
