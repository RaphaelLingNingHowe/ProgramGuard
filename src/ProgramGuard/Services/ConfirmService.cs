using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProgramGuard.Data;
using ProgramGuard.Interfaces;
using ProgramGuard.Models;
using System.Security.Claims;

namespace ProgramGuard.Services
{
    public class ConfirmService : IConfirmService
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<AppUser> _userManager;
        public ConfirmService(ApplicationDBContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task UpdateConfirmAsync(int id, ClaimsPrincipal currentUser)
        {
            var changeLog = await _context.ChangeLogs.FindAsync(id);

            // 確保找到了對應的異動檔案記錄
            if (changeLog != null)
            {
                // 更新異動檔案記錄的確認狀態和確認者/時間
                changeLog.ConfirmStatus = true;
                changeLog.ConfirmBy = currentUser.Identity.Name;
                changeLog.ConfirmTime = DateTime.UtcNow.ToLocalTime();

                await _context.SaveChangesAsync();
            }
        }


        public async Task<List<ChangeLog>> GetUnConfirmAsync()
        {
            return await _context.ChangeLogs.Where(cl => cl.ConfirmStatus == false).ToListAsync();
        }

    }
}
