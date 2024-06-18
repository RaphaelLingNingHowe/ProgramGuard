using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgramGuard.Data;
using ProgramGuard.Dtos.User;
using ProgramGuard.Interfaces;
using ProgramGuard.Models;
namespace ProgramGuard.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly ApplicationDBContext _context;
        private readonly ILogger<UserController> _logger;
        public UserController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService, ApplicationDBContext context, ILogger<UserController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _context = context;
            _logger = logger;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            try
            {

                var user = await _userManager.FindByNameAsync(loginDto.UserName);
                if (user == null)
                {
                    return BadRequest("用戶未找到");
                }
                if (user.IsFrozen)
                {
                    return BadRequest("賬號已凍結");
                }
                var result = await _signInManager.PasswordSignInAsync(loginDto.UserName, loginDto.Password, isPersistent: false, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    var daysSinceLastPasswordChange = (DateTime.UtcNow - user.LastPasswordChangedDate).TotalDays;
                    if (daysSinceLastPasswordChange > 80)
                    {
                        var userDto = new UserDto
                        {
                            UserName = loginDto.UserName,
                            RequirePasswordChange = true
                        };
                        return Ok(userDto);
                    }
                    user.LastLoginTime = DateTime.UtcNow.ToLocalTime();
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                    var token = await _tokenService.CreateTokenAsync(user);
                    return Ok(token);
                }
                else if (result.IsLockedOut)
                {
                    return BadRequest("賬號已鎖定，請稍後再試");
                }
                else
                {
                    return BadRequest("登錄失敗，請檢查賬號和密碼");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "登錄過程中發現異常");
                return StatusCode(500, "登錄失敗，請聯繫管理員");
            }
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var user = new AppUser
                {
                    UserName = registerDto.Username,
                    Email = registerDto.Email
                };
                var createdUser = await _userManager.CreateAsync(user, registerDto.Password);
                if (createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, "Admin");
                    if (roleResult.Succeeded)
                    {
                        return Ok(registerDto);
                    }
                    else
                    {
                        return StatusCode(500, roleResult.Errors);
                    }
                }
                else
                {
                    return StatusCode(500, createdUser.Errors);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "註冊用戶時發現異常");
                return StatusCode(500, "註冊用戶時發現異常");
            }
        }
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return Ok("用戶已註銷");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "用戶註銷時發送異常");
                return StatusCode(500, "用戶註銷時發送異常");
            }
        }
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("驗證失敗，請檢查輸入的格式");
                }
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    var userName = changePasswordDto.UserName;
                    user = await _userManager.FindByNameAsync(userName);
                }
                if (user == null)
                {
                    return BadRequest("未找到當前用戶");
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
                    return Ok("密碼已更改");
                }
                else
                {
                    return BadRequest("更改密碼失敗");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "在更改密碼時發送異常");
                return StatusCode(500, "在更改密碼時發送異常，請稍後再試");
            }
        }
    }
}
