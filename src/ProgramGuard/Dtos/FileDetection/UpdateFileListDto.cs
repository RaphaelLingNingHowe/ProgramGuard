using System.ComponentModel.DataAnnotations;

namespace ProgramGuard.Dtos.FileDetection
{
    public class UpdateFileListDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }

}
