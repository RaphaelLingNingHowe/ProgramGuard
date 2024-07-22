using ProgramGuard.Enums;

namespace ProgramGuard.Dtos.PrivilegeRule
{
    public class UpdatePrivilegeDto
    {
        public VISIBLE_PRIVILEGE Visible { get; set; }
        public OPERATE_PRIVILEGE Operate { get; set; }
    }
}
