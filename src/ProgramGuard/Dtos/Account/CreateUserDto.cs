using System.ComponentModel.DataAnnotations;
namespace ProgramGuard.Dtos.Account
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "請輸入賬號")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "請輸入電子郵件")]
        [EmailAddress(ErrorMessage = "請輸入有效的電子郵件地址")]
        public string Email { get; set; }

        [Required(ErrorMessage = "請輸入密碼")]
        [MinLength(8, ErrorMessage = "新密码长度不能少于8个字符")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "新密码必须包含大小写字母、数字和特殊字符")]
        public string Password { get; set; }
    }
}
