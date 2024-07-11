using ProgramGuard.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProgramGuard.Dtos.PrivilegeRule
{
    public class CreatePrivilegeDto
    {
        [Required(ErrorMessage = "請輸入規則名稱")]
        [StringLength(16, ErrorMessage = "長度不能超過16")]
        public string Name { get; set; }
        public VISIBLE_PRIVILEGE Visible { get; set; }
        public OPERATE_PRIVILEGE Operate { get; set; }
    }
}
