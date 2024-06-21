using System.ComponentModel.DataAnnotations;
namespace ProgramGuard.Dtos.Account
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "請輸入賬號")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "請輸入電子郵件")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "請輸入密碼")]
        public string Password { get; set; }
    }
}
