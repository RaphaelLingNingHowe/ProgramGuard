using ProgramGuard.Dtos.FileDetection;
using ProgramGuard.Models;
namespace ProgramGuard.Interfaces
{
    public interface IFileListRepository
    {
        Task<IEnumerable<FileList>> GetAllAsync();
        Task<FileList> AddAsync(FileList fileList);
        Task<FileList> UpdateAsync(int id, FileListModifyDto updateDto);
        Task<FileList> DeleteAsync(int id);
    }
}
