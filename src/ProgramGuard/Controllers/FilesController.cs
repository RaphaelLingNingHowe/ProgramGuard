using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgramGuard.Data;
using ProgramGuard.Dtos.FileDetection;
using ProgramGuard.Dtos.LogQuery;
using ProgramGuard.Interfaces;
using ProgramGuard.Mappers;
using ProgramGuard.Models;
using ProgramGuard.Repository;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ProgramGuard.Controllers
{
    [Route("[controller]")]
    [Authorize]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IFileDetectionService _fileDetectionService;
        private readonly ApplicationDBContext _context;
        private readonly IChangeLogRepository _changeLogRepository;
        private readonly IFileListRepository _fileListRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IActionLogRepository _actionLog;
        public FilesController(IFileDetectionService fileDetectionService, ApplicationDBContext context, IChangeLogRepository changeLogRepository, IFileListRepository fileListRepository, UserManager<AppUser> userManager, IActionLogRepository actionLog)
        {
            _fileDetectionService = fileDetectionService;
            _context = context;
            _changeLogRepository = changeLogRepository;
            _fileListRepository = fileListRepository;
            _userManager = userManager;
            _actionLog = actionLog;
        }

        [HttpGet]
        public async Task<IActionResult> GetFileLists()
        {
            var filelist = await _fileListRepository.GetAllAsync();
            // 將實體轉換為 DTO
            var fileDtos = filelist.Select(file => new FileListDto
            {
                Id = file.Id,
                FileName = file.FileName,
                FilePath = file.FilePath
            }).ToList();

            return Ok(fileDtos);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFileListAsync([FromBody] string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return BadRequest("File path is required.");
            }


            var existingFile = await _context.FileLists.FirstOrDefaultAsync(f => f.FilePath == filePath);
            var fileName = Path.GetFileName(filePath);
            FileList fileListModel;

            if (existingFile == null)
            {
                // 如果文件名不存在，则创建新的文件列表项
                var fileListDto = new FileListDto
                {
                    FilePath = filePath,
                    FileName = fileName
                };

                fileListModel = FileListMapper.FileListDtoToModel(fileListDto);

                // 将文件列表项添加到数据库
                await _fileListRepository.AddAsync(fileListModel);
            }
            else
            {
                // 如果文件名已存在，则获取现有文件列表项
                fileListModel = existingFile;
            }

            // 创建变更记录
            var isFileIntact = _fileDetectionService.VerifyFileIntegrity(filePath);

            if (!isFileIntact)
            {
                var currentMd5 = _fileDetectionService.CalculateMD5(filePath);
                var currentSha512 = _fileDetectionService.CalculateSHA512(filePath);
                var signature = _fileDetectionService.GetDigitalSignature(filePath);

                var changelog = new ChangeLogDTO
                {
                    FileName = fileName,
                    MD5 = currentMd5,
                    Sha512 = currentSha512,
                    DigitalSignature = signature,
                    ChangeTime = DateTime.UtcNow.ToLocalTime(),
                    ConfirmStatus = false,
                    ConfirmBy = null,
                    ConfirmTime = null,
                    FileListId = fileListModel.Id 
                };

                var changelogModel = ChangeLogMapper.ChangeLogDtoToModel(changelog);

                await _changeLogRepository.AddAsync(changelogModel);

                return Ok();
            }

            return Ok("No changes detected.");
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFileList(int id, [FromBody] FileListDto updateDto)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null)
            {
                var actionLog = new ActionLogDto
                {
                    UserName = currentUser.UserName,
                    Action = "Put File Lists"
                };

                var actionLogModel = ActionLogMapper.ActionLogDtoToModel(actionLog);
                await _actionLog.CreateAsync(actionLogModel);
            }
            try
            {
                var fileList = await _fileListRepository.UpdateAsync(id, updateDto);
                return Ok(fileList);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFileList([FromRoute] int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null)
            {
                var actionLog = new ActionLogDto
                {
                    UserName = currentUser.UserName,
                    Action = "Delete File Lists"
                };

                var actionLogModel = ActionLogMapper.ActionLogDtoToModel(actionLog);
                await _actionLog.CreateAsync(actionLogModel);
            }
            await _fileListRepository.DeleteAsync(id);
            return NoContent();
        }

    }
            
}
