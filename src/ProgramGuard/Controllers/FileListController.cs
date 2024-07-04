﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgramGuard.Base;
using ProgramGuard.Data;
using ProgramGuard.Dtos.FileDetection;
using ProgramGuard.Enums;
using ProgramGuard.Interfaces;
using ProgramGuard.Mappers;
using ProgramGuard.Models;
namespace ProgramGuard.Controllers
{
    [Route("[controller]")]
    [Authorize]
    [ApiController]
    public class FileListController : BaseController
    {
        private readonly IFileDetectionService _fileDetectionService;
        private readonly IChangeLogRepository _changeLogRepository;
        private readonly IFileListRepository _fileListRepository;

        public FileListController(IFileDetectionService fileDetectionService, ApplicationDBContext context, IChangeLogRepository changeLogRepository, IFileListRepository fileListRepository, UserManager<AppUser> userManager)
            : base(context, userManager)
        {
            _fileDetectionService = fileDetectionService;
            _changeLogRepository = changeLogRepository;
            _fileListRepository = fileListRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetFileLists()
        {
            var filelist = await _fileListRepository.GetAllAsync();
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
                return BadRequest("檔案路徑不可為空");
            }

            var existingFile = await _context.FileLists.FirstOrDefaultAsync(f => f.FilePath == filePath);
            if (existingFile != null)
            {
                return Conflict("檔案已經存在");
            }

            var fileName = Path.GetFileName(filePath);
            var fileListDto = new FileListDto
            {
                FilePath = filePath,
                FileName = fileName
            };
            var fileListModel = FileListMapper.FileListDtoToModel(fileListDto);
            await _fileListRepository.AddAsync(fileListModel);

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
                await LogActionAsync(ACTION.ADD_FILELIST);
                return Ok("檔案新增成功，已檢測到變更");
            }

            await LogActionAsync(ACTION.ADD_FILELIST);
            return Ok("檔案新增成功，未檢測到變更");
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateFileList(int id, [FromBody] FileListModifyDto updateDto)
        {

            try
            {
                var fileList = await _fileListRepository.UpdateAsync(id, updateDto);
                await LogActionAsync(ACTION.MODIFY_FILELIST);
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteFileList([FromRoute] int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            await _fileListRepository.DeleteAsync(id);
            await LogActionAsync(ACTION.DELETE_FILELIST);
            return NoContent();
        }
    }
}