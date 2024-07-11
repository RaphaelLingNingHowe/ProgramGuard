namespace ProgramGuard.Dtos.PrivilegeRule
{
    public class PrivilegesDto
    {
        public List<VisiblePrivilegeDto> VisiblePrivileges { get; set; }
        public List<OperatePrivilegeDto> OperatePrivileges { get; set; }
    }

    public class VisiblePrivilegeDto
    {
        public uint Value { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class OperatePrivilegeDto
    {
        public uint Value { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

}
