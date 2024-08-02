using System.ComponentModel.DataAnnotations;
namespace ProgramGuard.Dtos.FileDetection
{
    public class FileListDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "請輸入檔案名稱")]
        public string FileName { get; set; } = string.Empty;

        [Required(ErrorMessage = "請輸入路徑")]
        public string FilePath { get; set; } = string.Empty;
    }
}
