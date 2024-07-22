using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProgramGuard.Data;
using ProgramGuard.Enums;
using ProgramGuard.Models;
using System.Security.Claims;
namespace ProgramGuard.Base
{
    public class BaseController : ControllerBase
    {
        protected VISIBLE_PRIVILEGE? _visiblePrivilege;
        protected OPERATE_PRIVILEGE? _operatePrivilege;
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
            var actionLog = new ActionLog
            {
                User = currentUser,
                Action = action,
                Comment = comment,
                ActionTime = DateTime.UtcNow.ToLocalTime(),
            };

            _context.ActionLogs.Add(actionLog);
            await _context.SaveChangesAsync();
        }
        protected VISIBLE_PRIVILEGE VisiblePrivilege
        {
            get
            {
                if (_visiblePrivilege == null)
                {
                    if (User.Claims.FirstOrDefault(c => c.Type == "visiblePrivilege") is Claim rawClaims && uint.TryParse(rawClaims.Value, out uint privilege))
                    {
                        _visiblePrivilege = (VISIBLE_PRIVILEGE)privilege;
                    }
                    else
                    {
                        _visiblePrivilege = VISIBLE_PRIVILEGE.UNKNOWN;
                    }
                }

                return (VISIBLE_PRIVILEGE)_visiblePrivilege;
            }
        }
        protected OPERATE_PRIVILEGE OperatePrivilege
        {
            get
            {
                if (_operatePrivilege == null)
                {
                    if (User.Claims.FirstOrDefault(c => c.Type == "operatePrivilege") is Claim rawClaims && uint.TryParse(rawClaims.Value, out uint privilege))
                    {
                        _operatePrivilege = (OPERATE_PRIVILEGE)privilege;
                    }
                    else
                    {
                        _operatePrivilege = OPERATE_PRIVILEGE.UNKNOWN;
                    }
                }

                return (OPERATE_PRIVILEGE)_operatePrivilege;
            }
        }

        /* 沒有加 NonAction 會讓 Swagger 解析錯誤無法產生頁面 */

        /// <summary>
        /// 返回 HTTP 403 與失敗訊息
        /// </summary>
        /// <param name="value">失敗訊息</param>
        /// <returns>返回 HTTP 403 與失敗訊息</returns>
        [NonAction]
        public ObjectResult Forbidden(object value)
        {
            return StatusCode(403, value);
        }

        /// <summary>
        /// 返回 HTTP 201 與新增成功的資料識別項
        /// </summary>
        /// <param name="value">資料識別項</param>
        /// <returns>返回 HTTP 201 與新增成功的資料識別項</returns>
        [NonAction]
        public ObjectResult Created(object value)
        {
            return StatusCode(201, value);
        }

        /// <summary>
        /// 返回 HTTP 500 與失敗原因
        /// </summary>
        /// <param name="value">失敗原因</param>
        /// <returns>返回 HTTP 500 與失敗原因</returns>
        [NonAction]
        public ObjectResult ServerError(object value)
        {
            return StatusCode(500, value);
        }

        /// <summary>
        /// 返回 HTTP 400 與錯誤原因
        /// </summary>
        /// <param name="value">錯誤原因</param>
        /// <returns>返回 HTTP 400 與錯誤原因</returns>
        [NonAction]
        public new ObjectResult BadRequest(object value)
        {
            return StatusCode(400, value);
        }
    }
}