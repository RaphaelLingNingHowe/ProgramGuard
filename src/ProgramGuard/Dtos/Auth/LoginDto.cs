using System.ComponentModel.DataAnnotations;
namespace ProgramGuard.Dtos.User
{
    public class LoginDto
    {
        [Required(ErrorMessage = "請輸入帳號")]
        public string LoginUserName { get; set; }
        [Required(ErrorMessage = "請輸入密碼")]
        public string LoginPassword { get; set; }
    }
}
