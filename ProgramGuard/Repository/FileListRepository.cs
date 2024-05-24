using Microsoft.EntityFrameworkCore;
using ProgramGuard.Data;
using ProgramGuard.Interfaces;
using ProgramGuard.Models;

namespace ProgramGuard.Repository
{
    public class FileListRepository : Repository<FileList>, IFileListRepository
    {
        private readonly ApplicationDBContext _context;
        public FileListRepository (ApplicationDBContext context) : base (context)
        {
            _context = context;
        }

        // 根據檔案名稱查找文件清單
        public async Task<FileList> GetByNameAsync(string fileName)
        {
            return await _context.FileLists.FirstOrDefaultAsync(fl => fl.FileName == fileName);
        }

        // 獲取指定用戶擁有的所有文件清單
        //public async Task<IEnumerable<FileList>> GetByUserIdAsync(int userId)
        //{
        //    return await _context.FileLists.Where(fl => fl.UserId == userId).ToListAsync();
        //}

        // 添加一個新的文件清單
        public async Task AddFileListAsync(FileList fileList)
        {
            await _context.FileLists.AddAsync(fileList);
            await _context.SaveChangesAsync();
        }

        // 更新文件清單
        public async Task UpdateFileListAsync(FileList fileList)
        {
            _context.FileLists.Update(fileList);
            await _context.SaveChangesAsync();
        }

        // 刪除文件清單
        public async Task DeleteFileListAsync(int fileListId)
        {
            var fileList = await _context.FileLists.FindAsync(fileListId);
            if (fileList != null)
            {
                _context.FileLists.Remove(fileList);
                await _context.SaveChangesAsync();
            }
        }

        // 可以在這裡實現 FileList 特有的方法
    }
}
