using ProgramGuard.Dtos.Review;
using ProgramGuard.Models;
using System.Security.Claims;

namespace ProgramGuard.Interfaces
{
    public interface IReviewService
    {
        Task<List<ChangeLog>> GetUnreviewedItemsAsync();
        Task UpdateConfirmationAsync(int id, ClaimsPrincipal currentUser);
        //Task RejectItemAsync(int id);
    }
}
