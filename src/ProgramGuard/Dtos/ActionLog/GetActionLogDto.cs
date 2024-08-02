namespace ProgramGuard.Dtos.ActionLog
{
    public class GetActionLogDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string? Comment { get; set; }
        public DateTime ActionTime { get; set; } = DateTime.Now.ToLocalTime();
    }
}
