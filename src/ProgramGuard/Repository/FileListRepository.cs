using Microsoft.EntityFrameworkCore;
using ProgramGuard.Data;
using ProgramGuard.Dtos.FileList;
using ProgramGuard.Interfaces.Repository;
using ProgramGuard.Models;
using ProgramGuard.Repository;

namespace ProgramGuard.Repositories
{
    public class FileListRepository : Repository<FileList>, IFileListRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly ILogger<FileListRepository> _logger;

        public FileListRepository(ApplicationDBContext context,ILogger<FileListRepository> logger) : base(context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<FileList?> GetFileAsync(string path)
        {
            try
            {
                return await _context.FileLists
                    .AsNoTracking()
                    .FirstOrDefaultAsync(f => f.Path == path);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting file with path: {Path}", path);
                throw;
            }
        }

        public async Task<int> AddFileAsync(FileList file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            try
            {
                _context.FileLists.Add(file);
                await _context.SaveChangesAsync();
                return file.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding file: {FilePath}", file.Path);
                throw;
            }
        }

        public async Task<int> RenameFileAsync(string oldPath, string newPath)
        {
            try
            {
                var file = await GetFileAsync(oldPath);
                if (file == null)
                {
                    throw new FileNotFoundException("The file to be renamed was not found.", oldPath);
                }

                file.Path = newPath;
                file.Name = Path.GetFileName(newPath);
                _context.FileLists.Update(file);
                await _context.SaveChangesAsync();
                return file.Id;
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogWarning(ex, "File not found for renaming: {OldPath}", oldPath);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while renaming file from {OldPath} to {NewPath}", oldPath, newPath);
                throw;
            }
        }

        public async Task<int> DeleteFileAsync(string path)
        {
            try
            {
                var file = await GetFileAsync(path);
                if (file == null)
                {
                    throw new FileNotFoundException("The file to be deleted was not found.", path);
                }

                file.IsDeleted = true;
                _context.FileLists.Update(file);
                await _context.SaveChangesAsync();
                return file.Id;
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogWarning(ex, "File not found for deletion: {Path}", path);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting file: {Path}", path);
                throw;
            }
        }

        public async Task<IEnumerable<FileList>> GetAllFilesAsync()
        {
            try
            {
                return await _context.FileLists
                    .Where(f => !f.IsDeleted)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all files");
                throw;
            }

        }
    }
}