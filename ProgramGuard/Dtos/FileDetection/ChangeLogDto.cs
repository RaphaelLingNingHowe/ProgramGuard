using System.ComponentModel.DataAnnotations.Schema;

namespace ProgramGuard.Dtos.FileDetection
{
    public class ChangeLogDTO
    {
        public string FileName { get; set; }
        public string MD5 { get; set; }
        public string Sha512 { get; set; }
        public List<string>? DigitalSignature { get; set; }
        public DateTime ChangeTime { get; set; }
        public bool ConfirmationStatus { get; set; }
        public string? ConfirmedByAndTime { get; set; }
        public int FileListId { get; set; }

    }

}

