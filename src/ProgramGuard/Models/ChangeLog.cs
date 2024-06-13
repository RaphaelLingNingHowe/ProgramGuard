using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ProgramGuard.Models
{
    public class ChangeLog
    {
        public int Id { get; set; }
        [Required]
        [StringLength(255, ErrorMessage = "FileName cannot be longer than 255 characters.")]
        public string FileName { get; set; }
        [Required]
        public DateTime ChangeTime { get; set; }
        [Required]
        public bool ConfirmStatus { get; set; }
        [StringLength(255, ErrorMessage = "Reviewer cannot be longer than 255 characters.")]
        public string? ConfirmBy { get; set; }
        public DateTime? ConfirmTime { get; set; }
        // List<string> 无法直接使用 DataAnnotations，但你可以在业务逻辑中进行长度验证
        public List<string>? DigitalSignature { get; set; }
        [Required]
        [StringLength(32, ErrorMessage = "MD5 cannot be longer than 32 characters.")]
        public string MD5 { get; set; }
        [Required]
        [StringLength(128, ErrorMessage = "SHA512 cannot be longer than 128 characters.")]
        public string SHA512 { get; set; }
        [ForeignKey("FileListId")]
        [Required]
        public int FileListId { get; set; } // 外键，引用 FileList 表的 Id 字段
        [Required]
        public FileList FileList { get; set; } // 导航属性
    }
}
