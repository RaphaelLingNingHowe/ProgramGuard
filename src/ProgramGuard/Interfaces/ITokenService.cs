using ProgramGuard.Models;
namespace ProgramGuard.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(AppUser user);
    }
}
