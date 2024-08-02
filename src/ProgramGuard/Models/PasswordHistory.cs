using System.ComponentModel.DataAnnotations;
namespace ProgramGuard.Models
{
    public class PasswordHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public AppUser? User { get; set; }
    }
}
