namespace ProgramGuard.Dtos.LogQuery
{
    public class SearchDto
    {
        public string? FileName { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool CheckUnconfirm { get; set; }
    }
}
