using System.ComponentModel.DataAnnotations;
namespace ProgramGuard.Dtos.User
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "請輸入密碼")]
        public string CurrentPassword { get; set; }
        [Required(ErrorMessage = "請輸入新密碼")]
        [MinLength(8, ErrorMessage = "新密码长度不能少于8个字符")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "新密码必须包含大小写字母、数字和特殊字符")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "請輸入確認密碼")]
        [Compare("NewPassword", ErrorMessage = "新密碼和確認密碼不匹配")]
        public string ConfirmPassword { get; set; }
    }
}
