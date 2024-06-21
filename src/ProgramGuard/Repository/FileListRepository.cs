using Microsoft.EntityFrameworkCore;
using ProgramGuard.Data;
using ProgramGuard.Dtos.FileDetection;
using ProgramGuard.Interfaces;
using ProgramGuard.Models;
namespace ProgramGuard.Repository
{
    public class FileListRepository : IFileListRepository
    {
        private readonly ApplicationDBContext _context;
        public FileListRepository(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task<FileList> AddAsync(FileList fileList)
        {
            await _context.FileLists.AddAsync(fileList);
            await _context.SaveChangesAsync();
            return fileList;
        }
        public async Task<FileList> DeleteAsync(int id)
        {
            var fileList = await _context.FileLists.FindAsync(id);
            if (fileList != null)
            {
                _context.FileLists.Remove(fileList);
                await _context.SaveChangesAsync();
            }
            return null;
        }
        public async Task<IEnumerable<FileList>> GetAllAsync()
        {
            return await _context.FileLists.ToListAsync();
        }

        public async Task<FileList> UpdateAsync(int id, FileListModifyDto updateDto)
        {
            var fileList = await _context.FileLists.FindAsync(id);
            if (fileList == null)
            {
                throw new ArgumentException("File not found");
            }

            if (!string.IsNullOrEmpty(updateDto.FileName))
            {
                fileList.FileName = updateDto.FileName;
            }

            if (!string.IsNullOrEmpty(updateDto.FilePath))
            {
                fileList.FilePath = updateDto.FilePath;
            }

            await _context.SaveChangesAsync();
            return fileList;
        }

    }
}
