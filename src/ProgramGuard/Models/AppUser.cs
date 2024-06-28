using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ProgramGuard.Models
{
    public class AppUser : IdentityUser
    {
        public bool IsEnabled { get; set; }
        [Required]
        public short PrivilegeRule { get; set; }
        [Required]
        public DateTime LastPasswordChangedDate { get; set; } = DateTime.UtcNow.ToLocalTime();

    }
}
