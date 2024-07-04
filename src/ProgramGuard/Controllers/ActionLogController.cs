using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProgramGuard.Base;
using ProgramGuard.Data;
using ProgramGuard.Dtos.ActionLog;
using ProgramGuard.Enums;
using ProgramGuard.Interfaces;
using ProgramGuard.Models;
namespace ProgramGuard.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ActionLogController : BaseController
    {
        private readonly IActionLogRepository _actionLog;
        public ActionLogController(IActionLogRepository actionLog, UserManager<AppUser> userManager, ApplicationDBContext context) :base(context, userManager)
        {
            _actionLog = actionLog;
        }

        [HttpGet]
        public async Task<IActionResult> GetActionLogs([FromQuery] DateTime begin, [FromQuery] DateTime end)
        {
            if (begin > end)
            {
                return BadRequest("起始時間不能超過結束時間");
            }

            var actionLogs = await _actionLog.GetAsync(begin, end);
            await LogActionAsync(ACTION.VIEW_ACTION_LOG);
            return Ok(actionLogs);
        }
    }
}
