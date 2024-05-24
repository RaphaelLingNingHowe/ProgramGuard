namespace ProgramGuard.Models
{
    public class ActionLog
    {
        public int Id { get; set; }
        public string User { get; set; }
        public string Action { get; set; }
        public DateTime ActionTime { get; set; }

    }
}
