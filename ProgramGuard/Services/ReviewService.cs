using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProgramGuard.Data;
using ProgramGuard.Dtos.Review;
using ProgramGuard.Interfaces;
using ProgramGuard.Models;
using System.Security.Claims;

namespace ProgramGuard.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<User> _userManager;
        public ReviewService(ApplicationDBContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task UpdateConfirmationAsync(int id, ClaimsPrincipal currentUser)
        {
            var changeLog = await _context.ChangeLogs.FindAsync(id);

            // 確保找到了對應的異動檔案記錄
            if (changeLog != null)
            {
                // 更新異動檔案記錄的確認狀態和確認者/時間
                changeLog.ConfirmationStatus = true;
                changeLog.ConfirmedByAndTime = $"{currentUser.Identity.Name} at {DateTime.Now.ToLocalTime()}";

                // 將異動檔案記錄更新到資料庫中
                await _context.SaveChangesAsync();
            }
        }
      

        public async Task<List<ChangeLog>> GetUnreviewedItemsAsync()
        {
            return await _context.ChangeLogs.Where(cl => cl.ConfirmationStatus == false).ToListAsync();
        }

    }
}
