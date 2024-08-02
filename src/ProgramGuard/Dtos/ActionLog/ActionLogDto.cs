using ProgramGuard.Enums;

namespace ProgramGuard.Dtos.ActionLog
{
    public class ActionLogDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ACTION Action { get; set; }
        public string? Comment { get; set; }
        public DateTime ActionTime { get; set; } = DateTime.Now.ToLocalTime();
    }
}
