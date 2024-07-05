namespace ProgramGuard.Dtos.User
{
    public class RequirePasswordChangeDto
    {
        public string UserName { get; set; }
        public bool RequirePasswordChange { get; set; }
    }
}
