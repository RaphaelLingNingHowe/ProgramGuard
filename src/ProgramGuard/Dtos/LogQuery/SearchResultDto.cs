namespace ProgramGuard.Dtos.LogQuery
{
    public class SearchResultDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public DateTime ChangeTime { get; set; }
        public bool ConfirmationStatus { get; set; }
        public string? ConfirmedByAndTime { get; set; }
    }
}
