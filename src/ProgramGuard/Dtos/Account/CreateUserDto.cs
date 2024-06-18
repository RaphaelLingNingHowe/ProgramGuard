using System.ComponentModel.DataAnnotations;
namespace ProgramGuard.Dtos.Account
{
    public class CreateUserDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
