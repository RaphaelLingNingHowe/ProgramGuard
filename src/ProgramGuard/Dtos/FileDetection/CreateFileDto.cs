using System.ComponentModel.DataAnnotations;

namespace ProgramGuard.Dtos.FileDetection
{
    public class CreateFileDto
    {
        [Required(ErrorMessage = "請輸入檔案路徑")]
        [StringLength(4096, ErrorMessage = "超過可輸入上限(4096)")]
        [RegularExpression(@"^(?:[a-zA-Z]:\\|\\/)?(?:[\w\-. @\u4E00-\u9FFF]+[\\/])*[\w\-. @\u4E00-\u9FFF]*$", ErrorMessage = "無效的檔案路徑")]

        public string FilePath { get; set; } = string.Empty;
    }
}
