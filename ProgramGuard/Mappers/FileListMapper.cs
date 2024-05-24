using ProgramGuard.Dtos;
using ProgramGuard.Models;

namespace ProgramGuard.Mappers
{
    public static class FileListMapper
    {
        public static FileList FileListDtoToModel(FileListDto fileList)
        {
            return new FileList
            {
                Id = fileList.Id,
                FilePath = fileList.FilePath,
                FileName = fileList.FileName
            };
        }
    }
}
