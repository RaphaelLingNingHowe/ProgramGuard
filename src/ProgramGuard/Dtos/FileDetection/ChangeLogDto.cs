namespace ProgramGuard.Dtos.FileDetection
{
    public class ChangeLogDTO
    {
        public string FileName { get; set; }
        public string MD5 { get; set; }
        public string Sha512 { get; set; }
        public bool DigitalSignature { get; set; }
        public DateTime ChangeTime { get; set; }
        public bool ConfirmStatus { get; set; }
        public string? ConfirmBy { get; set; }
        public DateTime? ConfirmTime { get; set; }
        public int FileListId { get; set; }
    }
}
