using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgramGuard.Data;
using ProgramGuard.Dtos.LogQuery;
using ProgramGuard.Interfaces;
namespace ProgramGuard.Controllers
{
    [Route("[controller]")]
    [Authorize]
    [ApiController]
    public class ActionLogController : ControllerBase
    {
        private readonly ILogger<ActionLogController> _logger;       
        private readonly IActionLogRepository _actionLog;
        public ActionLogController(ILogger<ActionLogController> logger, IActionLogRepository actionLog)
        {
            _logger = logger;
            _actionLog = actionLog;
        }
        [HttpGet]
        public async Task<IActionResult> GetActionLogs()
        {
            var actionLog = await _actionLog.GetAllAsync();
            var actionLogDtos = actionLog.Select(a => new ActionLogDto
            {
                UserName = a.UserName,
                Action = a.Action,
                ActionTime = a.ActionTime,
            }).ToList();
            return Ok(actionLogDtos);
        }
    }
}
