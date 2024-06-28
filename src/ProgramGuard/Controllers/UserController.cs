using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgramGuard.Data;
using ProgramGuard.Dtos.Account;
using ProgramGuard.Dtos.User;
using ProgramGuard.Models;
using System.Text.Json;
namespace ProgramGuard.Controllers
{
    [Route("[controller]")]
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<UserController> _logger;
        public UserController(ApplicationDBContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<UserController> logger)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var userInfos = await _context.Users.ToListAsync();

                var userDtos = new List<GetUserDto>();

                foreach (var user in userInfos)
                {
                    var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

                    var userDto = new GetUserDto
                    {
                        UserId = user.Id,
                        UserName = user.UserName,
                        LockoutEnd = user.LockoutEnd?.LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        IsDisabled = user.IsDisabled,
                        IsAdmin = isAdmin
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
                };

                var createdUser = await _userManager.CreateAsync(user, createUserDto.Password);
                if (createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, "User");
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
                _logger.LogError(ex, "添加用戶時發生異常");
                return StatusCode(500, "伺服器發生問題，請稍後再試");
            }
        }

        [HttpPut("{userId}/toggleDisabled")]
        public async Task<IActionResult> ToggleDisabledAccount(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound("帳號未找到");
                }

                // 切换冻结状态
                user.IsDisabled = !user.IsDisabled;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    var message = user.IsDisabled ? "帳號已停用" : "帳號已啟用";
                    return Ok(message);
                }

                return BadRequest("操作失敗，請稍後再試");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "切換帳號停用狀態時發生異常");
                return StatusCode(500, "伺服器發生問題，請稍後再試");
            }
        }



        [HttpPut("{userId}/toggleAdmin")]
        public async Task<IActionResult> ToggleAdminRole(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound("帳號未找到");
                }

                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    // 用户已经是管理员，尝试移除管理员角色
                    var result = await _userManager.RemoveFromRoleAsync(user, "Admin");
                    if (result.Succeeded)
                    {
                        return Ok(new { message = "成功移除管理員" });
                    }
                    else
                    {
                        return StatusCode(500, $"移除管理員失敗: {string.Join(", ", result.Errors)}");
                    }
                }
                else
                {
                    // 用户不是管理员，尝试添加管理员角色
                    var result = await _userManager.AddToRoleAsync(user, "Admin");
                    if (result.Succeeded)
                    {
                        return Ok(new { message = "成功添加管理員" });
                    }
                    else
                    {
                        return StatusCode(500, $"管理員添加失敗: {string.Join(", ", result.Errors)}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "切換管理員時發生異常");
                return StatusCode(500, $"伺服器發生問題，請稍後再試: {ex.Message}");
            }
        }


        //[HttpPost]
        //public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        //{
        //    // 根据需要进行身份验证和授权检查

        //    // 查找用户
        //    var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);

        //    if (user == null)
        //    {
        //        return NotFound("User not found");
        //    }

        //    // 设置新密码（示例中简单地设置为新密码，实际中可能需要加密或其他安全措施）
        //    user.Password = model.NewPassword;

        //    // 更新数据库中的用户信息
        //    _context.Users.Update(user);
        //    await _context.SaveChangesAsync();

        //    return Ok("Password reset successfully");
        //}
    }
}
