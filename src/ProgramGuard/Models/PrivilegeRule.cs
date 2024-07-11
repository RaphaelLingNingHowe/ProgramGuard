using ProgramGuard.Enums;

namespace ProgramGuard.Models
{
    public class PrivilegeRule
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public VISIBLE_PRIVILEGE Visible { get; set; }
        public OPERATE_PRIVILEGE Operate { get; set; }
    }
}
