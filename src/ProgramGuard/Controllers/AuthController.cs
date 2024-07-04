using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgramGuard.Base;
using ProgramGuard.Data;
using ProgramGuard.Dtos.User;
using ProgramGuard.Enums;
using ProgramGuard.Interfaces;
using ProgramGuard.Models;
namespace ProgramGuard.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService, ApplicationDBContext context) : base(context, userManager)
        {
            _signInManager = signInManager;
            _tokenService = tokenService;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(loginDto.LoginUserName);
                if (user == null)
                {
                    return BadRequest("帳號不存在");
                }
                if (!user.IsEnabled)
                {
                    return BadRequest("帳號已停用");
                }
                var result = await _signInManager.PasswordSignInAsync(loginDto.LoginUserName, loginDto.LoginPassword, isPersistent: false, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    var daysSinceLastPasswordChange = (DateTime.UtcNow - user.LastPasswordChangedDate).TotalDays;
                    if (daysSinceLastPasswordChange > 80)
                    {
                        var userDto = new RequirePasswordChangeDto
                        {
                            UserName = loginDto.LoginUserName,
                            RequirePasswordChange = true
                        };
                        await LogActionAsync(ACTION.LOGIN,"需要更換密碼");
                        return Ok(userDto);
                    }
                    user.LastLoginTime = DateTime.UtcNow.ToLocalTime();
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                    var token = await _tokenService.CreateTokenAsync(user);
                    await LogActionAsync(ACTION.LOGIN);
                    return Ok(token);
                }
                else if (result.IsLockedOut)
                {
                    return BadRequest("帳號已鎖定，請稍後再試");
                }
                else
                {
                    return BadRequest("登錄失敗，請檢查帳號和密碼");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                await LogActionAsync(ACTION.LOGOUT);
                return Ok("用戶已註銷");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPut("{userId}/changePassword")]
        public async Task<IActionResult> ChangePassword(string userId, [FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("驗證失敗，請檢查輸入的格式");
                }
                var user = await _userManager.FindByNameAsync(userId);
                if (user == null)
                {
                    return BadRequest("帳號不存在");
                }
                var currentPasswordValid = await _userManager.CheckPasswordAsync(user, changePasswordDto.CurrentPassword);
                if (!currentPasswordValid)
                {
                    return BadRequest("當前密碼不正確");
                }
                var passwordHistories = await _context.PasswordHistories
                                        .Where(ph => ph.UserId == user.Id)
                                        .OrderByDescending(ph => ph.CreatedDate)
                                        .Take(3)
                                        .ToListAsync();
                foreach (var history in passwordHistories)
                {
                    if (_userManager.PasswordHasher.VerifyHashedPassword(user, history.PasswordHash, changePasswordDto.NewPassword) == PasswordVerificationResult.Success)
                    {
                        return BadRequest("新密碼不能與前三次使用的密碼相同");
                    }
                }
                var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
                if (result.Succeeded)
                {
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
                    return Ok("密碼已更改");
                }
                else
                {
                    return BadRequest("更改密碼失敗");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
