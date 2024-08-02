using ProgramGuard.Data;
using ProgramGuard.Interfaces.Repository;
using ProgramGuard.Models;

namespace ProgramGuard.Repositories
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
            await _context.AddAsync(changeLog);
            await _context.SaveChangesAsync();
        }
    }
}
