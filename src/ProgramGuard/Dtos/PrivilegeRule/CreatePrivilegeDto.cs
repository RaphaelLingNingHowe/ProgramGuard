using ProgramGuard.Enums;

namespace ProgramGuard.Dtos.PrivilegeRule
{
    public class CreatePrivilegeDto
    {
        public string Name { get; set; } = string.Empty;
        public VISIBLE_PRIVILEGE Visible { get; set; }
        public OPERATE_PRIVILEGE Operate { get; set; }
    }
}
