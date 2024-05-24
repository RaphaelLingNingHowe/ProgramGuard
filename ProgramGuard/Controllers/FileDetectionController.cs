using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgramGuard.Data;
using ProgramGuard.Dtos;
using ProgramGuard.Interfaces;
using ProgramGuard.Mappers;
using ProgramGuard.Models;
using ProgramGuard.Repository;

namespace ProgramGuard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileDetectionController : ControllerBase
    {
        private readonly IFileDetectionService _fileDetectionService;
        private readonly ApplicationDBContext _context;
        private readonly IChangeLogRepository _changeLogRepository;
        private readonly IFileListRepository _fileListRepository;

        public FileDetectionController(IFileDetectionService fileDetectionService, ApplicationDBContext context, IChangeLogRepository changeLogRepository, IFileListRepository fileListRepository)
        {
            _fileDetectionService = fileDetectionService;
            _context = context;
            _changeLogRepository = changeLogRepository;
            _fileListRepository = fileListRepository;
        }

        [HttpPost]
        public async Task<IActionResult> DetectFileAsync(string filePath)
        {
            //var result = _fileDetectionService.VerifyFileIntegrity(filePath);

            //if (result == false)
            //{

            //}




            var fileName = Path.GetFileName(filePath);
            var fileListDto = new FileListDto
            {
                FilePath = filePath,
                FileName = fileName
            };

            var fileListModel = FileListMapper.FileListDtoToModel(fileListDto);

            // 將 filelist 資料加入資料庫
            await _fileListRepository.AddAsync(fileListModel);

            // 確認 filelist 實體已保存並獲取到其主鍵 ID
            var savedFileList = await _context.FileLists.FirstOrDefaultAsync(f => f.FileName == fileName);

            if (savedFileList != null)
            {
                var md5 = _fileDetectionService.CalculateMD5(filePath);
                var sha512 = _fileDetectionService.CalculateSHA512(filePath);
                var signature = _fileDetectionService.GetDigitalSignature(filePath);

                var changelog = new ChangeLogDTO
                {
                    FileName = fileName,
                    MD5 = md5,
                    Sha512 = sha512,
                    DigitalSignature = signature,
                    ChangeTime = DateTime.UtcNow.ToLocalTime(),
                    ConfirmationStatus = true,
                    ConfirmedByAndTime = "TestUser - " + DateTime.Now,
                    FileListId = savedFileList.Id
                };

                var changelogModel = ChangeLogMapper.ChangeLogDtoToModel(changelog);

                // 將 changelog 資料加入資料庫
                await _changeLogRepository.AddAsync(changelogModel);

                // 返回測試結果
                return Ok(changelog);
            }
            else
            {
                // 如果無法獲取到 filelist 的主鍵 ID，則返回錯誤訊息
                return BadRequest("Unable to retrieve FileListId.");
            }
        }

        //[HttpGet("{filePath}")]
        //public IActionResult VerifyIntegrity(string filePath)
        //{
        //    var result = _fileDetectionService.VerifyFileIntegrity(filePath);
            

        //    if (result)
        //    {
        //        return Ok("File integrity verified.");
        //    }
        //    else
        //    {
        //        return BadRequest("File integrity verification failed.");
        //    }
        //}
    }
}
