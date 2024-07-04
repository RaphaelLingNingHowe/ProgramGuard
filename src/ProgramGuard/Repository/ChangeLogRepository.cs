using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProgramGuard.Data;
using ProgramGuard.Dtos.ActionLog;
using ProgramGuard.Dtos.LogQuery;
using ProgramGuard.Interfaces;
using ProgramGuard.Models;
using System.Security.Claims;
namespace ProgramGuard.Repository
{
    public class ChangeLogRepository : IChangeLogRepository
    {
        private readonly ApplicationDBContext _context;
        public ChangeLogRepository(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task AddAsync(ChangeLog changeLog)
        {
            await _context.ChangeLogs.AddAsync(changeLog);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<ChangeLog>> GetAllAsync()
        {
            return await _context.ChangeLogs.ToListAsync();
        }

        public async Task<List<GetChangeLogDto>> GetAsync(DateTime begin, DateTime end)
        {

            var query = await _context.ChangeLogs
                .Where(c => c.ChangeTime >= begin && c.ChangeTime <= end)
                .OrderByDescending(q => q.ChangeTime)
                .Select(c => new GetChangeLogDto
                {
                    Id = c.Id,
                    FileName = c.FileName,
                    ChangeTime = c.ChangeTime,
                    ConfirmStatus = c.ConfirmStatus,
                    ConfirmBy = c.ConfirmBy,
                    ConfirmTime = c.ConfirmTime
                })
                .ToListAsync();

            return query;
        }

        public async Task UpdateConfirmAsync(int id, string userId)
        {
            var changeLog = await _context.ChangeLogs.FindAsync(id);
            if (changeLog != null)
            {
                changeLog.ConfirmStatus = true;
                changeLog.ConfirmBy = userId; 
                changeLog.ConfirmTime = DateTime.UtcNow.ToLocalTime();
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"未找到 ID 為 {id} 的異動檔案記錄。");
            }
        }
    }
}
