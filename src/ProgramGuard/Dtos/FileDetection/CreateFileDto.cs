using System.ComponentModel.DataAnnotations;

namespace ProgramGuard.Dtos.FileDetection
{
    public class CreateFileDto
    {
        [Required(ErrorMessage = "請輸入檔案路徑")]
        [RegularExpression(@"^(?:[a-zA-Z]:|\\)\\(?:[\w\-. @\u4E00-\u9FFF]+\\)*[\w\-. @\u4E00-\u9FFF]+([\w.])*$", ErrorMessage = "無效的檔案路徑")]
        public string FilePath { get; set; }
    }
}
