namespace ProgramGuard.Dtos.Account
{
    public class GetUserDto
    {
        public string UserName { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public bool LockoutEnabled { get; set; }
        public string LockoutEnd { get; set; }
        public bool IsFrozen { get; set; }
    }
}
