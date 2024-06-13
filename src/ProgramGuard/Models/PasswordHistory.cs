using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace ProgramGuard.Models
{
    public class PasswordHistory
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        [JsonIgnore]
        public AppUser User { get; set; }
    }
}
