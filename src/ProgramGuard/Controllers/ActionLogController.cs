using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgramGuard.Base;
using ProgramGuard.Data;
using ProgramGuard.Dtos.ActionLog;
using ProgramGuard.Enums;
using ProgramGuard.Models;
namespace ProgramGuard.Controllers
{
    [Route("[controller]")]
    [Authorize]
    [ApiController]
    public class ActionLogController : BaseController
    {
        public ActionLogController(UserManager<AppUser> userManager, ApplicationDBContext context) : base(context, userManager)
        {
        }
        [HttpGet]
        public async Task<IActionResult> GetActionLogAsync([FromQuery] DateTime? startTime, DateTime? endTime)
        {
            if (VisiblePrivilege.HasFlag(VISIBLE_PRIVILEGE.SHOW_ACTION_LOG) == false)
            {
                return Forbid("沒有權限");
            }
            if (!startTime.HasValue && !endTime.HasValue)
            {
                return Ok(new List<GetActionLogDto>());
            }

            var query = _context.ActionLogs
                .Include(a => a.User)
                .AsQueryable();

            if (startTime.HasValue)
            {
                query = query.Where(e => e.ActionTime >= startTime);
            }

            if (endTime.HasValue)
            {
                query = query.Where(e => e.ActionTime <= endTime);
            }

            var logs = await query.ToListAsync();

            var result = logs.Select(log => new GetActionLogDto
            {
                Id = log.Id,
                UserId = log.User.UserName,
                Action = log.Action.GetDescription(),
                Comment = log.Comment,
                ActionTime = log.ActionTime
            });
            await LogActionAsync(ACTION.VIEW_ACTION_LOG, $"時間區間：{startTime:yyyy/MM/dd HH:mm}-{endTime:yyyy/MM/dd HH:mm}");
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> LogActionAsync(LogActionDto dto)
        {
            await LogActionAsync(dto.Action, dto.Comment);
            return Ok();
        }
    }
}
