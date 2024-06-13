using System.ComponentModel.DataAnnotations;
namespace ProgramGuard.Models
{
    public class FileList
    {
        public int Id { get; set; }
        [Required]
        [StringLength(255, ErrorMessage = "FileName cannot be longer than 255 characters.")]
        public string FileName { get; set; }
        [Required]
        [StringLength(4096, ErrorMessage = "FilePath cannot be longer than 4096 characters.")]
        public string FilePath { get; set; }
    }
}
