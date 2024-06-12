namespace ProgramGuard.Dtos.LogQuery
{
    public class ActionLogDto
    {
        public string UserName { get; set; }
        public string Action { get; set; }
        public DateTime ActionTime { get; set; } = DateTime.Now.ToLocalTime();
    }
}
