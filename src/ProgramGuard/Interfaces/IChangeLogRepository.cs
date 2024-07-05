using ProgramGuard.Dtos.LogQuery;
using ProgramGuard.Models;
namespace ProgramGuard.Interfaces
{
    public interface IChangeLogRepository
    {
        Task AddAsync(ChangeLog changeLog);
        Task<IEnumerable<ChangeLog>> GetAllAsync();
        Task<List<GetChangeLogDto>> GetAsync(DateTime begin, DateTime end);
        Task UpdateConfirmAsync(int id, string userId);
    }
}
