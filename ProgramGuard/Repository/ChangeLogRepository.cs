using Microsoft.EntityFrameworkCore;
using ProgramGuard.Data;
using ProgramGuard.Interfaces;
using ProgramGuard.Models;

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
        

    }
}
