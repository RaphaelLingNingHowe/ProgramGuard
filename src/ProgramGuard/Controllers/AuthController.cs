using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgramGuard.Base;
using ProgramGuard.Config;
using ProgramGuard.Data;
using ProgramGuard.Dtos.User;
using ProgramGuard.Enums;
using ProgramGuard.Interfaces.Service;
using ProgramGuard.Models;
namespace ProgramGuard.Controllers
{
    [AllowAnonymous]
    public class AuthController : BaseController
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService, ApplicationDBContext context, IConfiguration configuration) : base(context, userManager)
        {
            _signInManager = signInManager;
            _tokenService = tokenService;
            _configuration = configuration;
        }
        [HttpPost]
        public async Task<IActionResult> LockUser(string userId, DateTimeOffset? lockoutEnd)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // 使用自定义 UserManager 中的 SetLockoutEndDateAsync 方法
            var result = await _userManager.SetLockoutEndDateAsync(user, lockoutEnd);

            if (result.Succeeded)
            {
                return Ok();
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginDto loginDto)
        {
            if (await _userManager.FindByNameAsync(loginDto.LoginUserName) is not AppUser user)
            {
                return NotFound("帳號不存在");
            }

            if (!user.IsEnabled)
            {
                return Forbidden("帳號已停用");
            }

            await _context.Entry(user).Reference(u => u.PrivilegeRule).LoadAsync();

            var result = await _signInManager.PasswordSignInAsync(loginDto.LoginUserName, loginDto.LoginPassword, isPersistent: false, lockoutOnFailure: true);

            if (result.IsLockedOut)
            {
                var lockoutEnd = user.LockoutEnd?.UtcDateTime.ToLocalTime();
                return Forbidden($"帳號因多次登入失敗而被鎖定，請在{lockoutEnd}後再試");
            }
            if (!result.Succeeded)
            {
                return BadRequest("登入失敗，請檢查帳號和密碼");
            }
            if (user.LockoutEnd.HasValue && user.LockoutEnd.Value <= DateTimeOffset.Now)
            {
                await _userManager.SetLockoutEndDateAsync(user, null);
                return NoContent();
            }
            var passwordChangeDays = AppSettings.PasswordChangeDays;
            var daysSinceLastPasswordChange = (DateTime.UtcNow - user.LastPasswordChangedDate).TotalDays;
            if (daysSinceLastPasswordChange > passwordChangeDays)
            {
                var message = $"超過{passwordChangeDays}天未更換密碼，需要更換密碼";
                var loginResponseDto = new LoginResponseDto
                {
                    RequirePasswordChange = true,
                    Message = message
                };
                await LogActionAsync(ACTION.LOGIN, message);
                return Ok(loginResponseDto);
            }

            user.LastLoginTime = DateTime.UtcNow.ToLocalTime();
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            var token = await _tokenService.CreateTokenAsync(user);
            await LogActionAsync(ACTION.LOGIN);

            return Ok(new LoginResponseDto { Token = token });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            await LogActionAsync(ACTION.LOGOUT);

            return Ok();
        }

        [HttpPut("{userId}/changePassword")]
        public async Task<IActionResult> ChangePassword(string userId, [FromBody] ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("驗證失敗，請檢查輸入的格式");
            }
            var user = await _userManager.FindByNameAsync(userId);
            if (user == null)
            {
                return NotFound("帳號不存在");
            }
            var currentPasswordValid = await _userManager.CheckPasswordAsync(user, changePasswordDto.CurrentPassword);
            if (!currentPasswordValid)
            {
                return NotFound("當前密碼不正確");
            }
            var newPasswordHash = _userManager.PasswordHasher.HashPassword(user, changePasswordDto.NewPassword);
            var passwordHistoryCount = AppSettings.PasswordHistoryCount;
            bool isPasswordUsed = await _context.PasswordHistories
                                    .Where(ph => ph.UserId == user.Id)
                                    .OrderByDescending(ph => ph.CreatedDate)
                                    .Take(passwordHistoryCount)
                                    .AnyAsync(ph => ph.PasswordHash == newPasswordHash);
            if (isPasswordUsed)
            {
                return Forbidden("新密碼不能與前三次使用的密碼相同");
            }

            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
            if (!result.Succeeded)
            {
                return ServerError(result);
            }
            user.LastPasswordChangedDate = DateTime.UtcNow.ToLocalTime();
            await _userManager.UpdateAsync(user);

            var passwordHistory = new PasswordHistory
            {
                UserId = user.Id,
                PasswordHash = _userManager.PasswordHasher.HashPassword(user, changePasswordDto.NewPassword),
                CreatedDate = DateTime.UtcNow.ToLocalTime()
            };
            _context.PasswordHistories.Add(passwordHistory);
            await _context.SaveChangesAsync();

            await LogActionAsync(ACTION.CHANGE_PASSWORD);

            return NoContent();
        }
    }
}
