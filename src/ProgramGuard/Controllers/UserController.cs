using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgramGuard.Base;
using ProgramGuard.Data;
using ProgramGuard.Dtos.Account;
using ProgramGuard.Dtos.User;
using ProgramGuard.Enums;
using ProgramGuard.Models;
namespace ProgramGuard.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserController(ApplicationDBContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager) : base(context, userManager)
        {
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var userInfos = await _context.Users.Where(u => u.IsDeleted == false).OrderByDescending(u => u.LastLoginTime).ToListAsync();

                var userDtos = new List<GetUserDto>();

                foreach (var user in userInfos)
                {
                    var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                    var roles = await _userManager.GetRolesAsync(user);

                    var userDto = new GetUserDto
                    {
                        UserId = user.Id,
                        UserName = user.UserName,
                        LastLoginTime = user.LastLoginTime,
                        LockoutEnd = user.LockoutEnd?.LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        IsEnabled = user.IsEnabled,
                        IsAdmin = isAdmin,
                    };
                    userDtos.Add(userDto);
                }
                return Ok(userDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("addUser")]
        public async Task<IActionResult> AdminAddUser([FromBody] CreateUserDto createUserDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { Message = "驗證失敗", Errors = errors });
            }

            try
            {
                var existingUser = await _userManager.FindByNameAsync(createUserDto.UserName);
                if (existingUser != null)
                {
                    return BadRequest("用戶名已經存在，請選擇另一個用戶名");
                }

                var user = new AppUser
                {
                    UserName = createUserDto.UserName,
                    IsEnabled = true
                };

                var createdUser = await _userManager.CreateAsync(user, createUserDto.Password);
                if (createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, "Admin");
                    if (roleResult.Succeeded)
                    {
                        user.LastPasswordChangedDate = DateTime.UtcNow.ToLocalTime();
                        await _userManager.UpdateAsync(user);
                        var passwordHistory = new PasswordHistory
                        {
                            UserId = user.Id,
                            PasswordHash = _userManager.PasswordHasher.HashPassword(user, createUserDto.Password),
                            CreatedDate = DateTime.UtcNow.ToLocalTime()
                        };
                        _context.PasswordHistories.Add(passwordHistory);
                        await _context.SaveChangesAsync();
                        await LogActionAsync(ACTION.ADD_ACCOUNT, $"帳號 : {user.Id}");
                        return Ok("使用者創建成功");
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
                return StatusCode(500, "伺服器發生問題，請稍後再試");
            }
        }

        [HttpPut("{userId}/Active")]
        public async Task<IActionResult> ActiveAccountAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound("帳號未找到");
                }

                user.IsEnabled = true;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    await LogActionAsync(ACTION.ENABLE_ACCOUNT, $"帳號 : {userId}");
                    return Ok("帳號已啟用");
                }

                return BadRequest("操作失敗，請稍後再試");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "伺服器發生問題，請稍後再試");
            }
        }

        [HttpPut("{userId}/Disable")]
        public async Task<IActionResult> DisableAccountAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound("帳號未找到");
                }

                user.IsEnabled = false;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    await LogActionAsync(ACTION.DISABLE_ACCOUNT, $"帳號 : {userId}");
                    return Ok("帳號已停用");
                }

                return BadRequest("操作失敗，請稍後再試");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "伺服器發生問題，請稍後再試");
            }
        }

        [HttpPut("{userId}/SetAdmin")]
        public async Task<IActionResult> SetAdmin(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound("帳號未找到");
                }
                var result = await _userManager.AddToRoleAsync(user, "Admin");
                if (result.Succeeded)
                {
                    await LogActionAsync(ACTION.MODIFY_ROLE, $"帳號 : {userId}");
                    return Ok("成功添加管理員");
                }
                else
                {
                    return StatusCode(500, $"管理員添加失敗: {string.Join(", ", result.Errors)}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"伺服器發生問題，請稍後再試: {ex.Message}");
            }
        }

        [HttpPut("{userId}/RemoveAdmin")]
        public async Task<IActionResult> RemoveAdmin(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound("帳號未找到");
                }
                var result = await _userManager.RemoveFromRoleAsync(user, "Admin");
                if (result.Succeeded)
                {
                    await LogActionAsync(ACTION.MODIFY_ROLE, $"帳號 : {userId}");
                    return Ok("成功移除管理員");
                }
                else
                {
                    return StatusCode(500, $"移除管理員失敗: {string.Join(", ", result.Errors)}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"伺服器發生問題，請稍後再試: {ex.Message}");
            }
        }

        [HttpPost("{userId}/ResetPassword")]
        public async Task<IActionResult> ResetPassword(string userId, [FromBody] ResetPasswordDto resetPasswordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("驗證失敗，請檢查輸入的格式");
                }
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return BadRequest("帳號不存在");
                }
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, resetPasswordDto.ResetPassword);
                if (result.Succeeded)
                {
                    user.LastPasswordChangedDate = DateTime.UtcNow.ToLocalTime();
                    await _userManager.UpdateAsync(user);
                    var passwordHistory = new PasswordHistory
                    {
                        UserId = user.Id,
                        PasswordHash = _userManager.PasswordHasher.HashPassword(user, resetPasswordDto.ResetPassword),
                        CreatedDate = DateTime.UtcNow.ToLocalTime()
                    };
                    _context.PasswordHistories.Add(passwordHistory);
                    await _context.SaveChangesAsync();
                    await LogActionAsync(ACTION.RESET_PASSWORD, $"帳號 : {userId}");
                    return Ok("密碼已重置");
                }
                else
                {
                    return BadRequest("重置密碼失敗");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "在重置密碼時發送異常，請稍後再試");
            }
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteAccountAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound("帳號未找到");
                }

                user.IsDeleted = true;
                user.IsEnabled = false;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    await LogActionAsync(ACTION.DELETE_ACCOUNT, $"帳號 : {userId}");
                    return Ok("帳號已刪除");
                }

                return BadRequest("操作失敗，請稍後再試");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "伺服器發生問題，請稍後再試");
            }
        }
    }
}
