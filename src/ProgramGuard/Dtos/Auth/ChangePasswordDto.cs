using System.ComponentModel.DataAnnotations;
namespace ProgramGuard.Dtos.User
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "請輸入密碼")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "請輸入新密碼")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "新密碼必須包含大小寫字母、數字和特殊符號")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "請輸入確認密碼")]
        [Compare("NewPassword", ErrorMessage = "新密碼和確認密碼不匹配")]
        public string ConfirmPassword { get; set; }
    }
}
