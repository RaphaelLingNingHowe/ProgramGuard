using ProgramGuard.Dtos.ActionLog;
using ProgramGuard.Models;
namespace ProgramGuard.Interfaces
{
    public interface IActionLogRepository
    {
        Task<ActionLog> AddAsync(ActionLogDto actionLogDto);
        Task<IEnumerable<ActionLog>> GetAllAsync();
        Task<List<ActionLogDto>> GetAsync(DateTime begin, DateTime end);
    }
}
