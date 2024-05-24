using ProgramGuard.Data;
using ProgramGuard.Interfaces;
using ProgramGuard.Models;

namespace ProgramGuard.Repository
{
    public class ChangeLogRepository : Repository<ChangeLog>, IChangeLogRepository
    {
        private readonly ApplicationDBContext _context;
        public ChangeLogRepository(ApplicationDBContext context) : base(context) 
        {
            _context = context;
        }
        // 根據文件清單ID獲取相應的變更日誌
        //public async Task<IEnumerable<ChangeLog>> GetByFileListIdAsync(int fileListId)
        //{
        //    return await _context.ChangeLogs.Where(cl => cl.FileListId == fileListId).ToListAsync();
        //}

        // 添加一條新的變更日誌
        public async Task AddChangeLogAsync(ChangeLog changeLog)
        {
            await _context.ChangeLogs.AddAsync(changeLog);
            await _context.SaveChangesAsync();
        }

        // 更新變更日誌
        public async Task UpdateChangeLogAsync(ChangeLog changeLog)
        {
            _context.ChangeLogs.Update(changeLog);
            await _context.SaveChangesAsync();
        }

        // 刪除變更日誌
        public async Task DeleteChangeLogAsync(int changeLogId)
        {
            var changeLog = await _context.ChangeLogs.FindAsync(changeLogId);
            if (changeLog != null)
            {
                _context.ChangeLogs.Remove(changeLog);
                await _context.SaveChangesAsync();
            }
        }
    }
}
