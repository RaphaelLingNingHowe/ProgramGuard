using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        [HttpPost("search")]
        public async Task<IActionResult> SearchFilesAsync([FromBody] SearchDto searchDto)
        {
            var queryTime = DateTime.UtcNow.ToLocalTime();
            IQueryable<ChangeLog> query = _context.ChangeLogs.Include(cl => cl.FileList);
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
            var unconfirmed = await _confirm.GetUnConfirmAsync();
            return Ok(unconfirmed);
        }
        [HttpPut("confirm/{id}")]
        public async Task<IActionResult> UpdateConfirmAsync(int id)
        {
            if (!User.IsInRole("Admin"))
            {
                return Forbid();
            }
            var currentUser = await _userManager.GetUserAsync(User);
            var changeLog = await _context.ChangeLogs.FindAsync(id);
            if (changeLog != null)
            {
                changeLog.ConfirmStatus = true;
                changeLog.ConfirmBy = currentUser.UserName;
                changeLog.ConfirmTime = DateTime.UtcNow.ToLocalTime();
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
                return Ok();
            }
            return NotFound();
        }
    }
}
