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
    public class UserController : BaseController
    {
        public UserController(ApplicationDBContext context, UserManager<AppUser> userManager) : base(context, userManager)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersAsync()
        {
            if (VisiblePrivilege.HasFlag(VISIBLE_PRIVILEGE.SHOW_ACCOUNT_MANAGER) == false)
            {
                return Forbidden("沒有權限");
            }
            try
            {
                var users = await _context.Users
                    .Where(u => u.IsDeleted == false)
                    .OrderByDescending(u => u.LastLoginTime)
                    .Select(u => new GetUserDto
                    {
                        UserId = u.Id,
                        UserName = u.UserName ?? string.Empty,
                        LastLoginTime = u.LastLoginTime,
                        LockoutEnd = u.LockoutEnd.HasValue ? (DateTime?)u.LockoutEnd.Value.DateTime : null,
                        IsLocked = u.IsLocked,
                        IsEnabled = u.IsEnabled,
                        Privilege = u.Privilege

                    })
                    .ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                return ServerError(ex.Message);
            }
        }

        [HttpPost("createUser")]
        public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserDto createUserDto)
        {
            if (OperatePrivilege.HasFlag(OPERATE_PRIVILEGE.ADD_ACCOUNT) == false)
            {
                return Forbidden("沒有權限");
            }
            try
            {
                var existingUser = await _userManager.FindByNameAsync(createUserDto.UserName);
                if (existingUser != null)
                {
                    return Conflict("用戶名已經存在，請選擇另一個用戶名");
                }

                var user = new AppUser
                {
                    UserName = createUserDto.UserName,
                    IsEnabled = true,
                    Privilege = 1
                };

                var createdUser = await _userManager.CreateAsync(user, createUserDto.Password);
                if (createdUser.Succeeded)
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
                    return Created("使用者創建成功");
                }
                else
                {
                    return ServerError(createdUser.Errors);
                }
            }
            catch (Exception ex)
            {
                return ServerError(ex.Message);
            }
        }

        [HttpPut("{userId}/Active")]
        public async Task<IActionResult> ActiveAccountAsync(string userId)
        {
            if (OperatePrivilege.HasFlag(OPERATE_PRIVILEGE.ENABLE_ACCOUNT) == false)
            {
                return Forbidden("沒有權限");
            }
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
                    await LogActionAsync(ACTION.ENABLE_ACCOUNT, $"帳號 : {user.UserName}");
                    return Ok("帳號已啟用");
                }

                return ServerError(result.Errors);
            }
            catch (Exception ex)
            {
                return ServerError(ex.Message);
            }
        }

        [HttpPut("{userId}/Disable")]
        public async Task<IActionResult> DisableAccountAsync(string userId)
        {
            if (OperatePrivilege.HasFlag(OPERATE_PRIVILEGE.DISABLE_ACCOUNT) == false)
            {
                return Forbidden("沒有權限");
            }
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
                    await LogActionAsync(ACTION.DISABLE_ACCOUNT, $"帳號 : {user.UserName}");
                    return Ok("帳號已停用");
                }

                return BadRequest("操作失敗，請稍後再試");
            }
            catch (Exception ex)
            {
                return ServerError(ex.Message);
            }
        }

        [HttpPut("{userId}/privileges/{privilegeId}")]
        public async Task<IActionResult> UpdatePrivilegeAsync(string userId, int privilegeId)
        {
            if (OperatePrivilege.HasFlag(OPERATE_PRIVILEGE.MODIFY_PRIVILEGE) == false)
            {
                return Forbidden("沒有權限");
            }
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound("找不到此帳號");
                }
                var privilege = await _context.PrivilegeRules.FindAsync(privilegeId);
                if (privilege == null)
                {
                    return NotFound("找不到此權限");
                }

                user.Privilege = privilegeId;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    await LogActionAsync(ACTION.MODIFY_PRIVILEGE, $"帳號 : {user.UserName} , 權限 : {privilege.Name}");
                    return Ok("權限更換成功");
                }

                return BadRequest("操作失敗，請稍後再試");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"伺服器發生問題，請稍後再試: {ex.Message}");
            }
        }

        [HttpPut("{userId}/unlock")]
        public async Task<IActionResult> UnlockUser(string userId)
        {
            if (OperatePrivilege.HasFlag(OPERATE_PRIVILEGE.UNLOCK_ACCOUNT) == false)
            {
                return Forbidden("沒有權限");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            await _userManager.SetLockoutEndDateAsync(user, null);
            await LogActionAsync(ACTION.UNLOCK_ACCOUNT, $"帳號 : {user.UserName}");

            return NoContent();
        }


        [HttpPost("{userId}/ResetPassword")]
        public async Task<IActionResult> ResetPassword(string userId, [FromBody] ResetPasswordDto resetPasswordDto)
        {
            if (OperatePrivilege.HasFlag(OPERATE_PRIVILEGE.RESET_PASSWORD) == false)
            {
                return Forbidden("沒有權限");
            }
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
                    await LogActionAsync(ACTION.RESET_PASSWORD, $"帳號 : {user.UserName}");
                    return Ok("密碼已重置");
                }
                else
                {
                    return BadRequest("重置密碼失敗");
                }
            }
            catch (Exception ex)
            {
                return ServerError(ex.Message);
            }
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteAccountAsync(string userId)
        {
            if (OperatePrivilege.HasFlag(OPERATE_PRIVILEGE.DELETE_ACCOUNT) == false)
            {
                return Forbidden("沒有權限");
            }
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
                    await LogActionAsync(ACTION.DELETE_ACCOUNT, $"帳號 : {user.UserName}");
                    return NoContent();
                }

                return ServerError(result.Errors);
            }
            catch (Exception ex)
            {
                return ServerError(ex.Message);
            }
        }
    }
}
