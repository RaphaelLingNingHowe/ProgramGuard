using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ProgramGuard.Models
{
    public class AppUser : IdentityUser
    {
        public bool IsEnabled { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? LastLoginTime { get; set; }
        [Required]
        public DateTime LastPasswordChangedDate { get; set; } = DateTime.UtcNow.ToLocalTime();

        [Required]
        public int Privilege { get; set; }

        [ForeignKey("Privilege")]
        public PrivilegeRule PrivilegeRule { get; set; }

    }
}
