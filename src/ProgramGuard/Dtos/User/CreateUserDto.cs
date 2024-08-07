﻿using System.ComponentModel.DataAnnotations;
namespace ProgramGuard.Dtos.Account
{
    public class CreateUserDto
    {

        [Required(ErrorMessage = "請輸入帳號")]
        [MaxLength(16, ErrorMessage = "長度不能超過16")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "帳號只能包含字母和數字。")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "請輸入密碼")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "新密碼必須包含大小寫字母、數字和特殊符號")]
        public string Password { get; set; } = string.Empty;
    }
}
