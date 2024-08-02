namespace ProgramGuard.Dtos.Account
{
    public class GetUserDto
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime? LastLoginTime { get; set; }
        public DateTime? LockoutEnd { get; set; }
        public bool IsLocked { get; set; }
        public bool IsEnabled { get; set; }
        public int Privilege { get; set; }
    }
}
