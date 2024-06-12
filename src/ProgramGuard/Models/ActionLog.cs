using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgramGuard.Models
{
    public class ActionLog
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "UserName cannot be longer than 255 characters.")]
        public string UserName { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "Action cannot be longer than 255 characters.")]
        public string Action { get; set; }

        [Required]
        public DateTime ActionTime { get; set; }
    }
}
