using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProgramGuard.Data;
using ProgramGuard.Dtos.User;
using ProgramGuard.Interfaces;
using ProgramGuard.Models;
namespace ProgramGuard.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly ApplicationDBContext _context;
        private readonly ILogger<AuthController> _logger;
        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService, ApplicationDBContext context, ILogger<AuthController> logger)
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

                var user = await _userManager.FindByNameAsync(loginDto.LoginUserName);
                if (user == null)
                {
                    return BadRequest("用戶未找到");
                }
                if (user.IsFrozen)
                {
                    return BadRequest("賬號已凍結");
                }
                var result = await _signInManager.PasswordSignInAsync(loginDto.LoginUserName, loginDto.LoginPassword, isPersistent: false, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    var daysSinceLastPasswordChange = (DateTime.UtcNow - user.LastPasswordChangedDate).TotalDays;
                    if (daysSinceLastPasswordChange > 80)
                    {
                        var userDto = new UserDto
                        {
                            UserName = loginDto.LoginUserName,
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
    }
}
