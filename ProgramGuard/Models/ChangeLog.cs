using System.ComponentModel.DataAnnotations.Schema;

namespace ProgramGuard.Models
{
    public class ChangeLog
    {
        public int Id { get; set; }
        public DateTime ChangeTime { get; set; }
        public bool ConfirmationStatus { get; set; }
        public string ConfirmedByAndTime { get; set; }
        public List<string>? DigitalSignature { get; set; }
        public string FileName { get; set; }
        public string MD5 { get; set; }
        public string SHA512 { get; set; }

        [ForeignKey("FileListId")]
        public int FileListId { get; set; } // 外键，引用 FileList 表的 Id 字段
        public FileList FileList { get; set; } // 导航属性
    }
}
