using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProgramGuard.Data;
using ProgramGuard.Dtos.LogQuery;
using ProgramGuard.Interfaces;
using ProgramGuard.Mappers;
using ProgramGuard.Models;
namespace ProgramGuard.Controllers
{
    [Route("[controller]")]
    [Authorize]
    [ApiController]
    public class ChangeLogController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IQueryConditionHandler _query;
        private readonly UserManager<AppUser> _userManager;
        private readonly IActionLogRepository _actionLog;
        private readonly IChangeLogRepository _changeLog;
        private readonly IConfirmService _confirm;
        public ChangeLogController(ApplicationDBContext context, IQueryConditionHandler query, UserManager<AppUser> userManager, IActionLogRepository actionLog, IChangeLogRepository changeLog, IConfirmService confirm)
        {
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

        [HttpPut("confirm/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateConfirmAsync(int id)
        {
            try
            {
                if (!User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                var currentUser = await _userManager.GetUserAsync(User);
                var changeLog = await _context.ChangeLogs.FindAsync(id);

                changeLog.ConfirmStatus = true;
                changeLog.ConfirmBy = currentUser.UserName;
                changeLog.ConfirmTime = DateTime.UtcNow.ToLocalTime();

                await _context.SaveChangesAsync();

                if (currentUser != null)
                {
                    var actionLog = new ActionLogDto
                    {
                        UserName = currentUser.UserName,
                        Action = "審核異動記錄"
                    };
                    var actionLogModel = ActionLogMapper.ActionLogDtoToModel(actionLog);
                    await _actionLog.CreateAsync(actionLogModel);
                }

                return Ok("審核成功");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"審核失敗：{ex.Message}");
            }
        }

    }
}
