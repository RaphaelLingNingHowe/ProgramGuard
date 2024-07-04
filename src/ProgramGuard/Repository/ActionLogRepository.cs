using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProgramGuard.Data;
using ProgramGuard.Dtos.ActionLog;
using ProgramGuard.Interfaces;
using ProgramGuard.Models;

namespace ProgramGuard.Repository
{
    public class ActionLogRepository : IActionLogRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<AppUser> _userManager;
        public ActionLogRepository(ApplicationDBContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<ActionLog> AddAsync(ActionLogDto actionLogDto)
        {
            var actionLog = new ActionLog()
            {
                UserId = actionLogDto.UserId,
                Action = actionLogDto.Action,
                Comment = actionLogDto.Comment,
                ActionTime = actionLogDto.ActionTime
            };
            _context.ActionLogs.Add(actionLog);
            await _context.SaveChangesAsync();
            return actionLog;
        }
        public async Task<IEnumerable<ActionLog>> GetAllAsync()
        {
            return await _context.ActionLogs.ToListAsync();
        }

        public async Task<List<ActionLogDto>> GetAsync(DateTime begin, DateTime end)
        {
            var query = await _context.ActionLogs
                .Where(a => a.ActionTime >= begin && a.ActionTime <= end)
                .OrderByDescending(q => q.ActionTime)
                .Select(a => new ActionLogDto
                {
                    UserId = a.User.Id,
                    Action = a.Action,
                    Comment = a.Comment,
                    ActionTime = a.ActionTime,
                })
                .ToListAsync();

            return query;
        }
    }
}
