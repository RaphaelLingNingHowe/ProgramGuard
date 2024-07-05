namespace ProgramGuard.Dtos.ActionLog
{
    public class GetActionLogDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Action { get; set; }
        public string? Comment { get; set; }
        public DateTime ActionTime { get; set; } = DateTime.Now.ToLocalTime();
    }
}
