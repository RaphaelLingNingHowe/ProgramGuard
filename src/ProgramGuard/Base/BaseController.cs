﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProgramGuard.Data;
using ProgramGuard.Enums;
using ProgramGuard.Models;
namespace ProgramGuard.Base
{
    public class BaseController : ControllerBase
    {
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
    }
}