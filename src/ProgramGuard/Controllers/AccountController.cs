using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgramGuard.Data;
using ProgramGuard.Dtos.Account;
using ProgramGuard.Dtos.FileDetection;
using ProgramGuard.Dtos.User;
using ProgramGuard.Models;

namespace ProgramGuard.Controllers
{
    [Route("[controller]")]
    [Authorize]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ApplicationDBContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<AccountController> logger)
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
                var usersInfo = await (
                    from user in _context.Users
                        .Include(u => u.LoginHistories) //沒有userRole和role的導航屬性
                    join userRole in _context.UserRoles on user.Id equals userRole.UserId
                    join role in _context.Roles on userRole.RoleId equals role.Id
                    select new
                    {
                        User = user,
                        Role = role
                    }
                ).ToListAsync();

                var userDtos = usersInfo.Select(u => new GetUserDto
                {
                    //Id = u.User.Id,
                    UserName = u.User.UserName,
                    RoleName = u.Role.Name, 
                    LoginStatus = u.User.LoginHistories?.Any(lh => !lh.LogoutTime.HasValue) ?? false,
                    LoginTime = u.User.LoginHistories?.OrderByDescending(lh => lh.LoginTime).FirstOrDefault()?.LoginTime,
                    LogoutTime = u.User.LoginHistories?.OrderByDescending(lh => lh.LoginTime).FirstOrDefault()?.LogoutTime,
                    LockoutEnabled = u.User.LockoutEnabled,
                    LockoutEnd = u.User.LockoutEnd,
                    AccessFailedCount = u.User.AccessFailedCount,
                    IsFrozen = u.User.IsFrozen
                }).ToList();

                return Ok(userDtos);
            }
            catch (Exception ex)
            {
                // 可以根据实际情况记录日志或者返回适当的错误信息
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }




        [HttpPost("admin-add-user")]
        /*[Authorize(Roles = "Admin")] */// 只有管理员可以访问
        public async Task<IActionResult> AdminAddUser([FromBody]CreateUserDto createUserDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check if the username already exists
                var existingUser = await _userManager.FindByNameAsync(createUserDto.Username);
                if (existingUser != null)
                {
                    return BadRequest("用户名已经存在，请选择另一个用户名。");
                }

                var user = new AppUser
                {
                    UserName = createUserDto.Username,
                    Email = createUserDto.Email
                };

                var createdUser = await _userManager.CreateAsync(user, createUserDto.Password);

                if (createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, "User");
                    if (roleResult.Succeeded)
                    {
                        return Ok(createUserDto);
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
                _logger.LogError(ex, "An error occurred while adding user by admin");
                return StatusCode(500, "An error occurred while adding user by admin. Please try again later.");
            }
        }


        [HttpPost("freeze")]
        public async Task<IActionResult> FreezeAccount([FromBody] FreezeAccountDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
            {
                return NotFound("用户未找到");
            }

            user.IsFrozen = true;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok("账户已冻结");
            }

            return BadRequest("冻结账户失败");
        }

        [HttpPost("unfreeze")]
        public async Task<IActionResult> UnfreezeAccount([FromBody] FreezeAccountDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
            {
                return NotFound("用户未找到");
            }

            user.IsFrozen = false;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok("账户已解冻");
            }

            return BadRequest("解冻账户失败");
        }

    }
    
}
