using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ProgramGuard.Models
{
    public class ChangeLog
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string FileName { get; set; }

        [Required]
        public DateTime ChangeTime { get; set; }

        [Required]
        public bool ConfirmStatus { get; set; }

        public string? ConfirmBy { get; set; } 

        [ForeignKey("ConfirmBy")]
        public AppUser? AppUser { get; set; } // 導航屬性

        public DateTime? ConfirmTime { get; set; }

        public List<string>? DigitalSignature { get; set; }

        [Required]
        public string MD5 { get; set; }

        [Required]
        public string SHA512 { get; set; }

        [Required]
        public int FileListId { get; set; }

        [Required]
        [ForeignKey("FileListId")]
        public FileList FileList { get; set; } // 導航屬性
    }
}
