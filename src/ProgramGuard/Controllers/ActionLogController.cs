using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProgramGuard.Data;
using ProgramGuard.Dtos.FileDetection;
using ProgramGuard.Dtos.LogQuery;
using ProgramGuard.Interfaces;
using ProgramGuard.Models;

namespace ProgramGuard.Controllers
{
    [Route("[controller]")]
    [Authorize]
    [ApiController]
    public class ActionLogController : ControllerBase
    {
        private readonly ILogger<ActionLogController> _logger;
        private readonly ApplicationDBContext _context;
        private readonly IActionLogRepository _actionLog;
        public ActionLogController(ApplicationDBContext context, ILogger<ActionLogController> logger, IActionLogRepository actionLog)
        {
            _context = context;
            _logger = logger;
            _actionLog = actionLog;
        }

        [HttpGet]
        public async Task<IActionResult> GetActionLogs()
        {
            var actionLog = await _actionLog.GetAllAsync();
            // 將實體轉換為 DTO
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
