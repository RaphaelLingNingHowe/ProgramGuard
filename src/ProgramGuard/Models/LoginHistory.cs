using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace ProgramGuard.Models
{
    public class LoginHistory
    {
        [Key, ForeignKey("User")]
        public string UserId { get; set; }
        public bool LoginStatus { get; set; }  // 登录状态
        public DateTime? LoginTime { get; set; } // 登录时间
        public DateTime? LogoutTime { get; set; } // 登出时间
        [JsonIgnore]
        public AppUser User { get; set; }
    }
}
