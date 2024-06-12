namespace ProgramGuard.Dtos.Account
{
    public class GetUserDto
    {
        //public string Id { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }
        public bool LoginStatus { get; set; }
        public DateTime? LoginTime { get; set; } // 登录时间
        public DateTime? LogoutTime { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public int AccessFailedCount { get; set; }
        public bool IsFrozen { get; set; }

    }
}
