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

namespace ProgramGuard.Controllers
{
    [Route("[controller]")]
    [Authorize]
    [ApiController]
    public class ChangeLogController : ControllerBase
    {
        private readonly IFileDetectionService _fileDetectionService;
        private readonly ApplicationDBContext _context;
        private readonly IQueryConditionHandler _query;
        private readonly UserManager<AppUser> _userManager;
        private readonly IActionLogRepository _actionLog;
        private readonly IChangeLogRepository _changeLog;
        private readonly IConfirmService _confirm;

        public ChangeLogController(IFileDetectionService fileDetectionService, ApplicationDBContext context, IQueryConditionHandler query, UserManager<AppUser> userManager, IActionLogRepository actionLog, IChangeLogRepository changeLog, IConfirmService confirm)
        {
            _fileDetectionService = fileDetectionService;
            _context = context;
            _query = query;
            _userManager = userManager;
            _actionLog = actionLog;
            _changeLog = changeLog;
            _confirm = confirm;
        }

        [HttpGet]
        public async Task<IActionResult> GetChangeLogs()
        {
            var changeLog = await _changeLog.GetAllAsync();
            // 將實體轉換為 DTO
            var changeLogDtos = changeLog.Select(c => new GetChangeLogDto
            {
                Id = c.Id,
                FileName = c.FileName,
                ChangeTime = c.ChangeTime,
                ConfirmStatus = c.ConfirmStatus,
                ConfirmBy = c.ConfirmBy,
                ConfirmTime = c.ConfirmTime
            }).ToList();

            return Ok(changeLogDtos);
        }

        [HttpPost("search")]
        public async Task<IActionResult> SearchFilesAsync([FromBody] SearchDto searchDto)
        {
            var queryTime = DateTime.UtcNow.ToLocalTime();
            IQueryable<ChangeLog> query = _context.ChangeLogs.Include(cl => cl.FileList);


            // 使用 IQueryConditionHandler 處理查詢條件
            query = _query.HandleQueryConditions(query, searchDto);

            if (!query.Any())
            {
                return NotFound("No files found matching the specified criteria.");
            }

            var searchResults = await query.Select(cl => new SearchResultDto
            {
                Id = cl.Id,
                FileName = cl.FileName,
                ChangeTime = cl.ChangeTime,
                ConfirmStatus = cl.ConfirmStatus,
                ConfirmBy = cl.ConfirmBy,
                ConfirmTime = cl.ConfirmTime

            }).ToListAsync();

            if (searchResults.Count == 0)
            {
                return NotFound("No files found matching the specified criteria.");
            }
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser != null)
            {
                var actionLog = new ActionLogDto
                {
                    UserName = currentUser.UserName,
                    Action = "查詢" + searchDto.FileName,
                    ActionTime = queryTime
                };

                var actionLogModel = ActionLogMapper.ActionLogDtoToModel(actionLog);
                await _actionLog.CreateAsync(actionLogModel);
            }




            return Ok(searchResults);
        }

        [HttpPost("unconfirmed")]
        public async Task<IActionResult> GetUnconfirmedAsync()
        {
            var queryTime = DateTime.UtcNow.ToLocalTime();
            IQueryable<ChangeLog> query = _context.ChangeLogs.Include(cl => cl.FileList);

            // 使用 IQueryConditionHandler 處理查詢條件
            var unconfirmed = await _confirm.GetUnConfirmAsync();
            return Ok(unconfirmed);
        }

        [HttpPut("confirm/{id}")]
        public async Task<IActionResult> UpdateConfirmAsync(int id)
        {
            // 检查当前用户是否为管理员
            if (!User.IsInRole("Admin"))
            {
                return Forbid(); // 如果不是管理员，返回 403 Forbidden
            }
            // 從 HTTP 上下文中獲取當前用戶的身份信息
            var currentUser = await _userManager.GetUserAsync(User);

            var changeLog = await _context.ChangeLogs.FindAsync(id);

            // 確保找到了對應的異動檔案記錄
            if (changeLog != null)
            {
                // 更新異動檔案記錄的確認狀態和確認者/時間
                changeLog.ConfirmStatus = true;
                changeLog.ConfirmBy = currentUser.UserName;
                changeLog.ConfirmTime = DateTime.UtcNow.ToLocalTime();

                // 將異動檔案記錄更新到資料庫中
                await _context.SaveChangesAsync();

                if (currentUser != null)
                {
                    var actionLog = new ActionLogDto
                    {
                        UserName = currentUser.UserName,
                        Action = "審核"
                    };

                    var actionLogModel = ActionLogMapper.ActionLogDtoToModel(actionLog);
                    await _actionLog.CreateAsync(actionLogModel);
                }
                return Ok(); // 返回一個空的 Ok 結果
            }

            // 如果未找到對應的異動檔案記錄，返回 NotFound
            return NotFound();
        }





    }
}
