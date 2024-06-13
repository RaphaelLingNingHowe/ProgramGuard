using ProgramGuard.Models;
namespace ProgramGuard.Interfaces
{
    public interface IActionLogRepository
    {
        Task<ActionLog> CreateAsync(ActionLog actionLog);
        Task<IEnumerable<ActionLog>> GetAllAsync();
    }
}
