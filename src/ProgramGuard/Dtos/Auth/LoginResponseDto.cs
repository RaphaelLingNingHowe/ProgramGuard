namespace ProgramGuard.Dtos.User
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public bool RequirePasswordChange { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
