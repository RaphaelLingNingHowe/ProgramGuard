using System.ComponentModel.DataAnnotations;

namespace ProgramGuard.Models
{
    public class FileList
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "超過可輸入上限(255)")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(4096, ErrorMessage = "超過可輸入上限(4096)")]
        public string Path { get; set; } = string.Empty;

        [Required]
        public bool IsDeleted { get; set; }
    }
}
