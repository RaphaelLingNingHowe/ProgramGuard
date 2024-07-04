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

        [HttpGet]
        public async Task<IActionResult> GetActionLogs([FromQuery] DateTime startTime, [FromQuery] DateTime endTime, [FromQuery] string fileName, [FromQuery] bool unConfirmed)
        {
            if (begin > end)
            {
                return BadRequest("起始時間不能超過結束時間");
            }

            var actionLogs = await _changeLog.GetAsync(begin, end);
            await LogActionAsync(ACTION.ACCESS_CHANGELOG_PAGE);
            return Ok(actionLogs);
        }

        [HttpPut("confirm/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateConfirmAsync(int id)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                await _changeLog.UpdateConfirmAsync(id, userId);
                await LogActionAsync(ACTION.CONFIRM_CHANGELOG);
                return Ok("審核成功");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"審核失敗：{ex.Message}");
            }
        }

    }
}
