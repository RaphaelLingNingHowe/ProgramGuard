using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace ProgramGuard.Models
{
    public class PasswordHistory
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedDate { get; set; }
        [JsonIgnore]
        public AppUser User { get; set; }
    }
}
