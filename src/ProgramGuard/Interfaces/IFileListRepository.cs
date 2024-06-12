using System.Collections.Generic;
using System.Threading.Tasks;
using ProgramGuard.Dtos.FileDetection;
using ProgramGuard.Models;

namespace ProgramGuard.Interfaces
{
    public interface IFileListRepository
    {
        //Task<FileList> GetByIdAsync(int id);
        Task<IEnumerable<FileList>> GetAllAsync();
        Task<FileList> AddAsync(FileList fileList);
        Task<FileList> UpdateAsync(int id, FileListDto updateDto);
        Task<FileList> DeleteAsync(int id);
    }
}
