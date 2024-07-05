using System.ComponentModel.DataAnnotations;

namespace ProgramGuard.Dtos.User
{
    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "請輸入新密碼")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "新密碼必須包含大小寫字母、數字和特殊符號")]
        public string ResetPassword { get; set; }

    }
}
