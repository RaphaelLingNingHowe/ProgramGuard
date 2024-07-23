namespace ProgramGuard.Dtos.Account
{
    public class GetUserDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public string LockoutEnd { get; set; }
        public bool IsLocked { get; set; }
        public bool IsEnabled { get; set; }
        public int Privilege { get; set; }
    }
}
