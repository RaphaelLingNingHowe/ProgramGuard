namespace ProgramGuard.Dtos.PrivilegeRule
{
    public class PrivilegesDto
    {
        public List<VisiblePrivilegeDto> VisiblePrivileges { get; set; } = new List<VisiblePrivilegeDto>();
        public List<OperatePrivilegeDto> OperatePrivileges { get; set; } = new List<OperatePrivilegeDto>();
    }

    public class VisiblePrivilegeDto
    {
        public uint Value { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class OperatePrivilegeDto
    {
        public uint Value { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
