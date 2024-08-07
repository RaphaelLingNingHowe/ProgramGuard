﻿//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using ProgramGuard.Base;
//using ProgramGuard.Config;
//using ProgramGuard.Data;
//using ProgramGuard.Dtos.LogQuery;
//using ProgramGuard.Enums;
//using ProgramGuard.Interfaces;
//using ProgramGuard.Models;
//namespace ProgramGuard.Controllers
//{
//    public class ChangeLogController : BaseController
//    {
//        private readonly IChangeLogRepository _changeLog;
//        public ChangeLogController(ApplicationDBContext context, UserManager<AppUser> userManager, IChangeLogRepository changeLog) : base(context, userManager)
//        {
//            _changeLog = changeLog;
//        }

//        [HttpPut("confirm/{id}")]
//        public async Task<IActionResult> UpdateConfirmAsync(int id)
//        {
//            if (OperatePrivilege.HasFlag(OPERATE_PRIVILEGE.CONFIRM_CHANGE_LOG) == false)
//            {
//                return Forbidden("沒有權限");
//            }
//            try
//            {
//                var userId = _userManager.GetUserId(User);
//                if (userId == null)
//                {
//                    return Unauthorized();
//                }
//                await _changeLog.UpdateConfirmAsync(id, userId);
//                await LogActionAsync(ACTION.CONFIRM_CHANGE_LOG, $"檔案Id : {id}");
//                return Ok("審核成功");
//            }
//            catch (Exception ex)
//            {
//                return ServerError(ex.Message);
//            }
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetChangeLogAsync([FromQuery] DateTime? startTime, DateTime? endTime, string? fileName, bool? unConfirmed)
//        {
//            if (VisiblePrivilege.HasFlag(VISIBLE_PRIVILEGE.SHOW_CHANGE_LOG) == false)
//            {
//                return Forbidden("沒有權限");
//            }

//            if (!startTime.HasValue && !endTime.HasValue && string.IsNullOrEmpty(fileName) && !unConfirmed.HasValue)
//            {
//                return BadRequest("請至少提供一個查詢條件");
//            }
//            if (startTime.HasValue && endTime.HasValue)
//            {
//                if (!startTime.HasValue && !endTime.HasValue)
//                {
//                    return BadRequest("請提供查詢時間");
//                }
//                else if (endTime < startTime)
//                {
//                    return BadRequest("結束時間不能早於開始時間");
//                }
//                else if ((endTime - startTime).Value.TotalDays > AppSettings.MaxRangeInDays)
//                {
//                    return BadRequest($"時間範圍不能超過{AppSettings.MaxRangeInDays}天");
//                }
//            }

//            var query = _context.ChangeLogs
//                .Include(c => c.AppUser)
//                .AsQueryable();

//            if (startTime.HasValue)
//            {
//                query = query.Where(e => e.ChangeAt >= startTime);
//            }

//            if (endTime.HasValue)
//            {
//                query = query.Where(e => e.ChangeAt <= endTime);
//            }

//            if (!string.IsNullOrEmpty(fileName))
//            {
//                query = query.Where(e => e.FileListId.Contains(fileName));
//            }

//            if (unConfirmed == true)
//            {
//                query = query.Where(e => !e.ConfirmStatus);
//            }

//            var result = await query.Select(log => new GetChangeLogDto
//            {
//                Id = log.Id,
//                FileName = log.FileName,
//                ChangeAt = log.ChangeAt,
//                DigitalSignature = log.DigitalSignature,
//                ConfirmStatus = log.ConfirmStatus,
//                ConfirmBy = log.AppUser != null ? log.AppUser.UserName : string.Empty,
//                ConfirmTime = log.ConfirmTime
//            }).ToListAsync();


//            await LogActionAsync(ACTION.VIEW_CHANGE_LOG, $"時間區間：{startTime:yyyy/MM/dd HH:mm}-{endTime:yyyy/MM/dd HH:mm}");
//            return Ok(result);
//        }

//    }
//}
