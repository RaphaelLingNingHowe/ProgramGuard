using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProgramGuard.Data;
using ProgramGuard.Enums;
using ProgramGuard.Models;

namespace ProgramGuard.Base
{
    [Route("[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        // 與 protected VISIBLE_PRIVILEGE _visiblePrivilege; 等效, 因為 VISIBLE_PRIVILEGE.UNKNOWN 為預設值
        protected VISIBLE_PRIVILEGE _visiblePrivilege = VISIBLE_PRIVILEGE.UNKNOWN;
        protected OPERATE_PRIVILEGE _operatePrivilege = OPERATE_PRIVILEGE.UNKNOWN;
        protected readonly ApplicationDBContext _context;
        protected readonly UserManager<AppUser> _userManager;

        public BaseController(ApplicationDBContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        protected async Task LogActionAsync(ACTION action, string comment = "")
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                throw new InvalidOperationException("當前帳號未登入或無法識別");
            }
            ActionLog actionLog = new()
            {
                User = currentUser,
                Action = action,
                Comment = comment,
                Timestamp = DateTime.UtcNow
            };

            _context.ActionLogs.Add(actionLog);
            await _context.SaveChangesAsync();
        }

        protected VISIBLE_PRIVILEGE VisiblePrivilege
        {
            get
            {
                if (_visiblePrivilege == VISIBLE_PRIVILEGE.UNKNOWN)
                {
                    var claim = User.Claims.FirstOrDefault(c => c.Type == "visiblePrivilege");
                    if (claim != null && uint.TryParse(claim.Value, out uint privilege))
                    {
                        _visiblePrivilege = (VISIBLE_PRIVILEGE)privilege;
                    }
                }

                return _visiblePrivilege;
            }
        }

        protected OPERATE_PRIVILEGE OperatePrivilege
        {
            get
            {
                if (_operatePrivilege == OPERATE_PRIVILEGE.UNKNOWN)
                {
                    var claim = User.Claims.FirstOrDefault(c => c.Type == "operatePrivilege");
                    if (claim != null && uint.TryParse(claim.Value, out uint privilege))
                    {
                        _operatePrivilege = (OPERATE_PRIVILEGE)privilege;
                    }
                }

                return _operatePrivilege;
            }
        }

        [NonAction]
        public ObjectResult Forbidden(object value)
        {
            return StatusCode(403, value);
        }

        [NonAction]
        public ObjectResult Created(object value)
        {
            return StatusCode(201, value);
        }

        [NonAction]
        public ObjectResult ServerError(object value)
        {
            return StatusCode(500, value);
        }

        [NonAction]
        public new ObjectResult BadRequest(object value)
        {
            return StatusCode(400, value);
        }
    }
}
