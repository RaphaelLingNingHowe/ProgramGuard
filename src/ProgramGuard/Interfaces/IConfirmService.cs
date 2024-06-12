using ProgramGuard.Dtos.Review;
using ProgramGuard.Models;
using System.Security.Claims;

namespace ProgramGuard.Interfaces
{
    public interface IConfirmService
    {
        Task<List<ChangeLog>> GetUnConfirmAsync();
        Task UpdateConfirmAsync(int id, ClaimsPrincipal currentUser);
    }
}
