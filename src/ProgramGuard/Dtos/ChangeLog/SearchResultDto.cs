namespace ProgramGuard.Dtos.LogQuery
{
    public class SearchResultDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public DateTime ChangeTime { get; set; }
        public bool ConfirmStatus { get; set; }
        public string? ConfirmBy { get; set; }
        public DateTime? ConfirmTime { get; set; }
    }
}
