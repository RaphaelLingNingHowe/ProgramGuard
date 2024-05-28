using System.ComponentModel.DataAnnotations;

namespace ProgramGuard.Dtos.FileDetection
{
    public class UpdateFileListDto
    {
        [Required(ErrorMessage = "The FileName field is required.")]
        [MaxLength(100, ErrorMessage = "The FileName field cannot exceed 100 characters.")]
        public string FileName { get; set; }

        [Required(ErrorMessage = "The FilePath field is required.")]
        [MaxLength(100, ErrorMessage = "The FilePath field cannot exceed 100 characters.")]
        public string FilePath { get; set; }
    }

}
