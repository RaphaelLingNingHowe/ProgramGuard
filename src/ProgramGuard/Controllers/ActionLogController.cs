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
            if ((endTime - startTime).TotalDays > 7)
            {
                return BadRequest("時間範圍不能超過7天");
            }

            var query = _context.ActionLogs
                .Include(a => a.User)
                .Where(a => a.ActionTime >= startTime && a.ActionTime <= endTime)
                .OrderByDescending(a => a.ActionTime);

            var result = await query
                .Select(a => new GetActionLogDto
                {
                    Id = a.Id,
                    UserId = a.User.UserName,
                    Action = a.Action.GetDescription(),
                    Comment = a.Comment,
                    ActionTime = a.ActionTime
                })
                .ToListAsync();

            await LogActionAsync(ACTION.VIEW_ACTION_LOG, $"時間區間：{startTime:yyyy/MM/dd HH:mm}-{endTime:yyyy/MM/dd HH:mm}");

            return Ok(result);
        }


        [HttpPost]
        public async Task<IActionResult> LogActionAsync(LogActionDto dto)
        {
            await LogActionAsync(dto.Action, dto.Comment);
            return Created();
        }
    }
}
