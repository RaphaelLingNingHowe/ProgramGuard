using ProgramGuard.Models;
using System.Threading.Tasks;

namespace ProgramGuard.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(User user);
    }
}
