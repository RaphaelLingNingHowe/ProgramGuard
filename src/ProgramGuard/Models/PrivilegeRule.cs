using ProgramGuard.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProgramGuard.Models
{
    public class PrivilegeRule
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public VISIBLE_PRIVILEGE Visible { get; set; }

        [Required]
        public OPERATE_PRIVILEGE Operate { get; set; }
    }
}
