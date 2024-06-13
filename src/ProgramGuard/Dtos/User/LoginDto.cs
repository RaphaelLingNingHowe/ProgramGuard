using System.ComponentModel.DataAnnotations;
namespace ProgramGuard.Dtos.User
{
    public class LoginDto
    {
        [Required(ErrorMessage = "請輸入賬號")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "請輸入密碼")]
        public string Password { get; set; }
    }
}
