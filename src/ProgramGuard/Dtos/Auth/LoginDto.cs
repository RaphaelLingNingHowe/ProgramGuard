using System.ComponentModel.DataAnnotations;
namespace ProgramGuard.Dtos.User
{
    public class LoginDto
    {

        [Required(ErrorMessage = "請輸入帳號")]
        [StringLength(16, ErrorMessage = "超過可輸入上限(16)")]
        public string LoginUserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "請輸入密碼")]
        [StringLength(256, ErrorMessage = "超過可輸入上限(256)")]
        public string LoginPassword { get; set; } = string.Empty;
    }
}
