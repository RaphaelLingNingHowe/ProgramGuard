using Microsoft.EntityFrameworkCore;
using ProgramGuard.Data;
using ProgramGuard.Interfaces;
using ProgramGuard.Models;

namespace ProgramGuard.Repository
{
    public class ActionLogRepository : IActionLogRepository
    {
        private readonly ApplicationDBContext _context;
        public ActionLogRepository(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task<ActionLog> CreateAsync(ActionLog actionLog)
        {
            _context.ActionLogs.Add(actionLog);
            await _context.SaveChangesAsync();
            return actionLog;
        }
    }
}
