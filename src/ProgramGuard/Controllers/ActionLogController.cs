using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgramGuard.Base;
using ProgramGuard.Config;
using ProgramGuard.Data;
using ProgramGuard.Dtos.ActionLog;
using ProgramGuard.Enums;
using ProgramGuard.Models;
namespace ProgramGuard.Controllers
{
    public class ActionLogController : BaseController
    {
        private readonly IConfiguration _configuration;
        public ActionLogController(UserManager<AppUser> userManager, ApplicationDBContext context, IConfiguration configuration) : base(context, userManager)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetActionLogAsync([FromQuery] DateTime startTime, DateTime endTime)
        {
            if (VisiblePrivilege.HasFlag(VISIBLE_PRIVILEGE.SHOW_ACTION_LOG) == false)
            {
                return Forbidden("沒有權限");
            }
            if (endTime < startTime)
            {
                return BadRequest("結束時間不能早於開始時間");
            }
            if ((endTime - startTime).TotalDays > AppSettings.MaxRangeInDays)
            {
                return BadRequest($"時間範圍不能超過{AppSettings.MaxRangeInDays}天");
            }

            var query = _context.ActionLogs
            .Include(a => a.User)
            .Where(a => a.Timestamp >= startTime && a.Timestamp <= endTime)
            .OrderByDescending(a => a.Timestamp);

            IEnumerable<GetActionLogDto> result = await query.Select(a => new GetActionLogDto
            {
                Id = a.Id,
                UserId = a.User == null ? "Unknown" :
                         string.IsNullOrEmpty(a.User.UserName) ? "Unknown" :
                         a.User.UserName,
                Action = a.Action.GetDescription(),
                Comment = a.Comment,
                ActionTime = a.Timestamp
            }).ToListAsync();


            await LogActionAsync(ACTION.VIEW_ACTION_LOG, $"時間區間：{startTime:yyyy/MM/dd HH:mm}-{endTime:yyyy/MM/dd HH:mm}");

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> LogActionAsync(LogActionDto dto)
        {
            await LogActionAsync(dto.Action);
            return Created();
        }
    }
}
