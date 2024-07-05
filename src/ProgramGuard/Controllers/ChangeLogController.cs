using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgramGuard.Base;
using ProgramGuard.Data;
using ProgramGuard.Dtos.LogQuery;
using ProgramGuard.Enums;
using ProgramGuard.Interfaces;
using ProgramGuard.Models;
namespace ProgramGuard.Controllers
{
    [Route("[controller]")]
    [Authorize]
    [ApiController]
    public class ChangeLogController : BaseController
    {
        private readonly IChangeLogRepository _changeLog;
        public ChangeLogController(ApplicationDBContext context, UserManager<AppUser> userManager, IChangeLogRepository changeLog) : base(context, userManager)
        {
            _changeLog = changeLog;
        }

        [HttpPut("confirm/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateConfirmAsync(int id)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                await _changeLog.UpdateConfirmAsync(id, userId);
                await LogActionAsync(ACTION.CONFIRM_CHANGE_LOG, $"檔案Id : {id}");
                return Ok("審核成功");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"審核失敗：{ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetChangeLogAsync([FromQuery] DateTime? startTime, DateTime? endTime, string fileName, bool? unConfirmed)
        {
            if (!startTime.HasValue && !endTime.HasValue && string.IsNullOrEmpty(fileName) && !unConfirmed.HasValue)
            {
                return Ok(new List<GetChangeLogDto>());
            }

            var query = _context.ChangeLogs
                .Include(c => c.AppUser)
                .AsQueryable();

            if (startTime.HasValue)
            {
                query = query.Where(e => e.ChangeTime >= startTime);
            }

            if (endTime.HasValue)
            {
                query = query.Where(e => e.ChangeTime <= endTime);
            }

            if (!string.IsNullOrEmpty(fileName))
            {
                query = query.Where(e => e.FileName.Contains(fileName));
            }

            if (unConfirmed == true)
            {
                query = query.Where(e => !e.ConfirmStatus);
            }

            var logs = await query.ToListAsync();

            var result = logs.Select(log => new GetChangeLogDto
            {
                Id = log.Id,
                FileName = log.FileName,
                ChangeTime = log.ChangeTime,
                DigitalSignature = log.DigitalSignature,
                ConfirmStatus = log.ConfirmStatus,
                ConfirmBy = log.AppUser != null ? log.AppUser.UserName : string.Empty,
                ConfirmTime = log.ConfirmTime
            }).ToList();
            await LogActionAsync(ACTION.VIEW_CHANGE_LOG, $"時間區間：{startTime:yyyy/MM/dd HH:mm}-{endTime:yyyy/MM/dd HH:mm}");
            return Ok(result);
        }
    }
}
