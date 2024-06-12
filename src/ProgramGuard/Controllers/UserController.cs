using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProgramGuard.Data;
using ProgramGuard.Dtos.User;
using ProgramGuard.Interfaces;
using ProgramGuard.Models;
using System.Security.Claims;

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
                if (!ModelState.IsValid)
                {
                    return BadRequest("验证失败，请检查输入的格式");
                }

                var user = await _userManager.FindByNameAsync(loginDto.UserName);
                if (user == null)
                {
                    return BadRequest("用户未找到");
                }

                if (user.IsFrozen)
                {
                    return BadRequest("账户已被冻结");
                }

                var result = await _signInManager.PasswordSignInAsync(loginDto.UserName, loginDto.Password, isPersistent: false, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    // 检查上次密码更改日期
                    var daysSinceLastPasswordChange = (DateTime.UtcNow - user.LastPasswordChangedDate).TotalDays;
                    if (daysSinceLastPasswordChange > 1)
                    {
                        return Ok(new { Message = "密码已超过80天未更改，请更改密码" });
                    }

                    var existingHistory = await _context.LoginHistories.FirstOrDefaultAsync(h => h.UserId == user.Id);
                    if (existingHistory != null)
                    {
                        // 如果存在，更新现有记录
                        existingHistory.LoginStatus = true;
                        existingHistory.LoginTime = DateTime.UtcNow.ToLocalTime();
                        _context.LoginHistories.Update(existingHistory);
                    }
                    else
                    {
                        // 如果不存在，插入新记录
                        var loginHistory = new LoginHistory
                        {
                            UserId = user.Id,
                            LoginStatus = true,
                            LoginTime = DateTime.UtcNow.ToLocalTime()
                        };
                        _context.LoginHistories.Add(loginHistory);
                    }

                    await _context.SaveChangesAsync();

                    var token = await _tokenService.CreateTokenAsync(user);
                    return Ok(new { Token = token });
                }
                else if (result.IsLockedOut)
                {
                    return BadRequest("账号已锁定，请稍后再试");
                }
                else
                {
                    return BadRequest("登录失败，请检查用户名和密码");
                }
            }
            catch (Exception ex)
            {
                // 记录异常日志
                _logger.LogError(ex, "登录过程中发生异常");
                return StatusCode(500, "登录失败，请联系管理员");
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
                // 记录异常信息
                _logger.LogError(ex, "注册用户时发生异常");

                return StatusCode(500, "注册用户时发生异常");
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();

                // 记录用户的登出时间
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    var loginHistory = await _context.LoginHistories.FindAsync(userId);
                    if (loginHistory != null)
                    {
                        loginHistory.LoginStatus = false;
                        loginHistory.LogoutTime = DateTime.UtcNow.ToLocalTime();
                        _context.LoginHistories.Update(loginHistory);
                        await _context.SaveChangesAsync();
                    }
                }

                return Ok("用户已注销");
            }
            catch (Exception ex)
            {
                // 记录异常信息
                _logger.LogError(ex, "用户注销时发生异常");

                return StatusCode(500, "用户注销时发生异常");
            }
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("验证失败，请检查输入的格式");
                }
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    var userName = changePasswordDto.UserName;
                    user = await _userManager.FindByNameAsync(userName); // 获取当前登录用户
                }                
                
                if (user == null)
                {
                    return BadRequest("未找到当前用户");
                }

                var currentPasswordValid = await _userManager.CheckPasswordAsync(user, changePasswordDto.CurrentPassword);
                if (!currentPasswordValid)
                {
                    return BadRequest("当前密码不正确");
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
                        return BadRequest("新密码不能与前三次使用的密码相同");
                    }
                }

                var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
                if (result.Succeeded)
                {
                    user.LastPasswordChangedDate = DateTime.UtcNow.ToLocalTime(); // 记录更改密码的时间
                    await _userManager.UpdateAsync(user);

                    var passwordHistory = new PasswordHistory
                    {
                        UserId = user.Id,
                        PasswordHash = _userManager.PasswordHasher.HashPassword(user, changePasswordDto.NewPassword),
                        CreatedDate = DateTime.UtcNow.ToLocalTime()
                    };

                    _context.PasswordHistories.Add(passwordHistory); // 密码记录历史
                    await _context.SaveChangesAsync();

                    return Ok("密码已更改");
                }
                else
                {
                    return BadRequest("更改密码失败");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "在更改密码时发生异常");
                return StatusCode(500, "在更改密码时发生异常，请稍后再试");
            }
        }

        [HttpPost("change-password-authorized")]
        [Authorize]
        public async Task<IActionResult> ChangePasswordAuthorized(ChangePasswordDto changePasswordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("验证失败，请检查输入的格式");
                }
                var user = await _userManager.GetUserAsync(User);              

                if (user == null)
                {
                    return BadRequest("未找到当前用户");
                }

                var currentPasswordValid = await _userManager.CheckPasswordAsync(user, changePasswordDto.CurrentPassword);
                if (!currentPasswordValid)
                {
                    return BadRequest("当前密码不正确");
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
                        return BadRequest("新密码不能与前三次使用的密码相同");
                    }
                }

                var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
                if (result.Succeeded)
                {
                    user.LastPasswordChangedDate = DateTime.UtcNow.ToLocalTime(); // 记录更改密码的时间
                    await _userManager.UpdateAsync(user);

                    var passwordHistory = new PasswordHistory
                    {
                        UserId = user.Id,
                        PasswordHash = _userManager.PasswordHasher.HashPassword(user, changePasswordDto.NewPassword),
                        CreatedDate = DateTime.UtcNow.ToLocalTime()
                    };

                    _context.PasswordHistories.Add(passwordHistory); // 密码记录历史
                    await _context.SaveChangesAsync();

                    return Ok("密码已更改");
                }
                else
                {
                    return BadRequest("更改密码失败");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "在更改密码时发生异常");
                return StatusCode(500, "在更改密码时发生异常，请稍后再试");
            }
        }

    }
}
