using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgramGuard.Data;
using ProgramGuard.Dtos.FileDetection;
using ProgramGuard.Interfaces;
using ProgramGuard.Mappers;
using ProgramGuard.Models;
using ProgramGuard.Repository;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ProgramGuard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IFileDetectionService _fileDetectionService;
        private readonly ApplicationDBContext _context;
        private readonly IChangeLogRepository _changeLogRepository;
        private readonly IFileListRepository _fileListRepository;

        public FilesController(IFileDetectionService fileDetectionService, ApplicationDBContext context, IChangeLogRepository changeLogRepository, IFileListRepository fileListRepository)
        {
            _fileDetectionService = fileDetectionService;
            _context = context;
            _changeLogRepository = changeLogRepository;
            _fileListRepository = fileListRepository;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var filelist = await _fileListRepository.GetAllAsync();
            return Ok(filelist);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DetectFileAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return BadRequest("File path is required.");
            }

            var existingFile = await _context.FileLists.FirstOrDefaultAsync(f => f.FilePath == filePath);
            var fileName = Path.GetFileName(filePath);
            if (existingFile == null)
            {
                
                var fileListDto = new FileListDto
                {
                    FilePath = filePath,
                    FileName = fileName
                };

                var fileListModel = FileListMapper.FileListDtoToModel(fileListDto);

                // 將 filelist 資料加入資料庫
                await _fileListRepository.AddAsync(fileListModel);
            }
            // 確認 filelist 實體已保存並獲取到其主鍵 ID
            var savedFileList = await _context.FileLists.FirstOrDefaultAsync(f => f.FileName == fileName);
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
                    ConfirmationStatus = false,
                    ConfirmedByAndTime = null,
                    FileListId = savedFileList.Id
                };

                var changelogModel = ChangeLogMapper.ChangeLogDtoToModel(changelog);

                await _changeLogRepository.AddAsync(changelogModel);

                return Ok(changelog);
            }
            return Ok("No changes detected.");
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFileList(int id, [FromBody] UpdateFileListDto updateDto)
        {
            try
            {
                var updatedFileList = await _fileListRepository.UpdateAsync(id, updateDto);
                return Ok(updatedFileList);
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

        [Authorize]
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await _fileListRepository.DeleteAsync(id);
            return NoContent();
        }

    }
            
}
