using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProgramGuard.Base;
using ProgramGuard.Data;
using ProgramGuard.Dtos.FileList;
using ProgramGuard.Dtos.PathRequest;
using ProgramGuard.Interfaces.Repository;
using ProgramGuard.Interfaces.Service;
using ProgramGuard.Models;
namespace ProgramGuard.Controllers
{
    public class FileListController : BaseController
    {
        private readonly IPathProcessService _pathProcessService;
        private readonly IFileListRepository _fileListRepository;
        public FileListController(ApplicationDBContext context, UserManager<AppUser> userManager, IPathProcessService pathProcessService,IFileListRepository fileListRepository)
            : base(context, userManager)
        {
            _fileListRepository = fileListRepository;
            _pathProcessService = pathProcessService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllFilesAsync()
        {
            var files = await _fileListRepository.GetAllFilesAsync();
            return Ok(files);
        }

        [HttpPost]
        public async Task<IActionResult> AddPathToLists([FromBody] PathRequestDto pathRequestDto)
        {
            await _pathProcessService.ProcessPathAsync(pathRequestDto.Path);

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFileAsync(int id, [FromBody] FileListUpdateDto fileUpdateDto)
        {
            var fileList = await _fileListRepository.GetByIdAsync(id);
            if (fileList == null)
            {
                return NotFound(); 
            }
            fileList.Path = fileUpdateDto.Path;
            fileList.Name = fileUpdateDto.Name;
            try
            {
                await _fileListRepository.UpdateAsync(fileList);
                return Ok();
            }
            catch (Exception ex)
            {
                return ServerError(ex);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFileAsync(int id)
        {
            var fileList = await _fileListRepository.GetByIdAsync(id);
            if (fileList == null)
            {
                return NotFound();
            }
            fileList.IsDeleted = true;
            try
            {
                await _fileListRepository.UpdateAsync(fileList);
                return Ok();
            }
            catch (Exception ex)
            {
                return ServerError(ex);
            }
        }


    }
}
