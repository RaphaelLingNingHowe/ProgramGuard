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
                        IsEnabled = user.IsEnabled,
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

        [HttpPut("{userId}/toggleFreeze")]
        public async Task<IActionResult> ToggleFreezeAccount(string userId, [FromBody] JsonElement json)
        {
            try
            {
                bool IsEnabled = json.GetProperty("IsEnabled").GetBoolean();
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound("用戶未找到");
                }

                if (user.IsEnabled == IsEnabled)
                {
                    return Ok($"用戶已經{(IsEnabled ? "凍結" : "解凍")}");
                }

                user.IsEnabled = IsEnabled; // 根據參數設置凍結狀態

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    if (user.IsEnabled)
                    {
                        return Ok("賬號已凍結");
                    }
                    else
                    {
                        return Ok("賬號已解凍");
                    }
                }

                return BadRequest("操作失敗，請稍後再試");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "切换账户冻结状态时发生异常");
                return StatusCode(500, "伺服器發生問題，請稍後再試");
            }
        }


        [HttpPut("{userId}/toggleAdmin")]
        public async Task<IActionResult> ToggleAdminRole(string userId, [FromBody] JsonElement json)
        {
            try
            {
                bool IsAdmin = json.GetProperty("IsAdmin").GetBoolean();
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound("找不到指定的用戶");
                }

                if (IsAdmin)
                {
                    if (await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        return Ok(new { message = "用戶已經是管理員" });
                    }

                    var result = await _userManager.AddToRoleAsync(user, "Admin");
                    if (result.Succeeded)
                    {
                        return Ok(new { message = "成功添加管理員" });
                    }
                    else
                    {
                        return StatusCode(500, $"管理員添加失敗: {result.Errors}");
                    }
                }
                else
                {
                    if (!await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        return Ok(new { message = "用戶已經不是管理員" });
                    }

                    var result = await _userManager.RemoveFromRoleAsync(user, "Admin");
                    if (result.Succeeded)
                    {
                        return Ok(new { message = "成功移除管理員" });
                    }
                    else
                    {
                        return StatusCode(500, $"移除管理員失敗: {result.Errors}");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        
    }
}
