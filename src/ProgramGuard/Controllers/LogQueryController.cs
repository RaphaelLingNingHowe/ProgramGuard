using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgramGuard.Data;
using ProgramGuard.Dtos.LogQuery;
using ProgramGuard.Dtos.Review;
using ProgramGuard.Interfaces;
using ProgramGuard.Mappers;
using ProgramGuard.Models;

namespace ProgramGuard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogQueryController : ControllerBase
    {
        private readonly IFileDetectionService _fileDetectionService;
        private readonly ApplicationDBContext _context;
        private readonly IQueryConditionHandler _query;
        private readonly UserManager<User> _userManager;
        private readonly IActionLogRepository _actionLog;
        private readonly IChangeLogRepository _changeLog;
        private readonly IReviewService _review;

        public LogQueryController(IFileDetectionService fileDetectionService, ApplicationDBContext context, IQueryConditionHandler query, UserManager<User> userManager, IActionLogRepository actionLog, IChangeLogRepository changeLog, IReviewService review)
        {
            _fileDetectionService = fileDetectionService;
            _context = context;
            _query = query;
            _userManager = userManager;
            _actionLog = actionLog;
            _changeLog = changeLog;
            _review = review;
        }

        [Authorize]
        [HttpPost("search")]
        public async Task<IActionResult> SearchFilesAsync([FromBody] LogQueryDto queryDto)
        {
            var queryTime = DateTime.UtcNow.ToLocalTime();
            IQueryable<ChangeLog> query = _context.ChangeLogs.Include(cl => cl.FileList);

            // 使用 IQueryConditionHandler 處理查詢條件
            query = _query.HandleQueryConditions(query, queryDto);

            var searchResults = await query.ToListAsync();

            if (searchResults.Count == 0)
            {
                return NotFound("No files found matching the specified criteria.");
            }
            var currentUser = await _userManager.GetUserAsync(User);

            var actionLog = new ActionLogDto
            {
                UserName = currentUser.UserName,
                Action = "查詢" + queryDto.FileName,
                ActionTime = queryTime
            };

            var actionLogModel = ActionLogMapper.ActionLogDtoToModel(actionLog);
            await _actionLog.CreateAsync(actionLogModel);


            return Ok(searchResults);
        }

        [Authorize]
        [HttpPost("unconfirmed")]
        public async Task<IActionResult> GetUnconfirmedAsync()
        {
            var queryTime = DateTime.UtcNow.ToLocalTime();
            IQueryable<ChangeLog> query = _context.ChangeLogs.Include(cl => cl.FileList);

            // 使用 IQueryConditionHandler 處理查詢條件
            var unreviewed = await _review.GetUnreviewedItemsAsync();
            return Ok(unreviewed);
        }

        [Authorize]
        [HttpPut("{id}/confirm")]
        public async Task<IActionResult> UpdateConfirmationAsync(int id)
        {
            // 從 HTTP 上下文中獲取當前用戶的身份信息
            var currentUser = await _userManager.GetUserAsync(User);

            var changeLog = await _context.ChangeLogs.FindAsync(id);

            // 確保找到了對應的異動檔案記錄
            if (changeLog != null)
            {
                // 更新異動檔案記錄的確認狀態和確認者/時間
                changeLog.ConfirmationStatus = true;
                changeLog.ConfirmedByAndTime = $"{currentUser.UserName} - {DateTime.Now.ToLocalTime()}";

                // 將異動檔案記錄更新到資料庫中
                await _context.SaveChangesAsync();

                return Ok(); // 返回一個空的 Ok 結果
            }

            // 如果未找到對應的異動檔案記錄，返回 NotFound
            return NotFound();
        }





    }
}
