using System.ComponentModel.DataAnnotations;

namespace ProgramGuard.Dtos.FileDetection
{
    public class CreateFileListsDto
    {
        [Required]
        public string FilePath { get; set; }
    }
}
