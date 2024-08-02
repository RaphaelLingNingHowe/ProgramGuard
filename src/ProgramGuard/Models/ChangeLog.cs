using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProgramGuard.Models
{
    public class ChangeLog
    {
        public int Id { get; set; }

        [Required]
        public int FileListId { get; set; }

        [ForeignKey(nameof(FileListId))]
        public FileList? FileList { get; set; }

        [Required]
        public DateTime Timestamp { get; set; } = DateTime.Now;

        [Required]
        public byte? ChangeType { get; set; }

        public string? ChangeDetails { get; set; }

        [Required]
        public bool IsConfirmed { get; set; }

        public string? ConfirmedBy { get; set; }

        [ForeignKey(nameof(ConfirmedBy))]
        public AppUser? AppUser { get; set; }

        public DateTime? ConfirmedAt { get; set; }

        [Required]
        public bool DigitalSignature { get; set; }

        public string? SHA512 { get; set; } = string.Empty;
    }
}
