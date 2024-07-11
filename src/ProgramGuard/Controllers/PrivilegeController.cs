using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgramGuard.Base;
using ProgramGuard.Data;
using ProgramGuard.Dtos.PrivilegeRule;
using ProgramGuard.Enums;
using ProgramGuard.Models;
using ProgramGuard.Services;

namespace ProgramGuard.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PrivilegeController : BaseController
    {
        public PrivilegeController(UserManager<AppUser> userManager, ApplicationDBContext context) : base(context, userManager)
        {
        }

        [HttpGet]
        public async Task<ActionResult> GetPrivilegesAsync(string? contain)
        {
            try
            {
                IQueryable<PrivilegeRule> query = _context.PrivilegeRules;

                if (!string.IsNullOrEmpty(contain))
                {
                    query = query.Where(o => o.Name.Contains(contain));
                }

                IEnumerable<GetPrivilegeRuleDto> result = await query
                    .Select(o => new GetPrivilegeRuleDto
                    {
                        Id = o.Id,
                        Name = o.Name,
                        Visible = o.Visible,
                        Operate = o.Operate
                    })
                    .ToListAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"伺服器發生問題，請稍後再試: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddPrivilegeRuleAsync(CreatePrivilegeDto createPrivilegeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("驗證失敗，請檢查輸入的格式");
            }

            try
            {
                if (await _context.PrivilegeRules.AnyAsync(p => p.Name == createPrivilegeDto.Name))
                {
                    return BadRequest("權限名稱已被使用");
                }

                var existingPrivilegeRule = await _context.PrivilegeRules.SingleOrDefaultAsync(o => o.Visible == createPrivilegeDto.Visible && o.Operate == createPrivilegeDto.Operate);

                if (existingPrivilegeRule != null)
                {
                    return BadRequest("權限規則已存在");
                }

                var privilege = new PrivilegeRule()
                {
                    Name = createPrivilegeDto.Name,
                    Visible = createPrivilegeDto.Visible,
                    Operate = createPrivilegeDto.Operate
                };

                await _context.PrivilegeRules.AddAsync(privilege);
                await _context.SaveChangesAsync();
                return Ok("權限創建成功");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"伺服器發生問題，請稍後再試: {ex.Message}");
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdatePrivilegeRule(int id, UpdatePrivilegeDto updatePrivilegeDto)
        {
            try
            {
                var privilegeRule = await _context.PrivilegeRules.FindAsync(id);
                if (privilegeRule == null)
                {
                    return NotFound($"找不到此權限規則[{id}]");
                }

                if (await _context.PrivilegeRules.AnyAsync(p => p.Name == updatePrivilegeDto.Name && p.Id != id))
                {
                    return BadRequest("權限名稱已被使用");
                }

                var existingPrivilegeRule = await _context.PrivilegeRules.SingleOrDefaultAsync(o => o.Visible == updatePrivilegeDto.Visible && o.Operate == updatePrivilegeDto.Operate && o.Id != id);
                if (existingPrivilegeRule != null)
                {
                    return BadRequest($"已具有相同規則{existingPrivilegeRule.Name}");
                }

                privilegeRule.Name = updatePrivilegeDto.Name;
                privilegeRule.Visible = updatePrivilegeDto.Visible;
                privilegeRule.Operate = updatePrivilegeDto.Operate;

                _context.PrivilegeRules.Update(privilegeRule);
                await _context.SaveChangesAsync();

                return Ok("權限規則更新成功");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"伺服器發生問題，請稍後再試: {ex.Message}");
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                var privilegeRule = await _context.PrivilegeRules.FindAsync(id);
                if (privilegeRule == null)
                {
                    return NotFound($"找不到此權限規則[{id}]");
                }

                if (await _context.Users.AnyAsync(o => o.Privilege == id))
                {
                    return BadRequest($"此規則已被帳號引用、無法刪除");
                }

                _context.PrivilegeRules.Remove(privilegeRule);

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"伺服器發生問題，請稍後再試: {ex.Message}");
            }
        }
        [HttpGet("/Privileges")]
        public IActionResult GetPrivileges()
        {
            var visiblePrivileges = Enum.GetValues(typeof(VISIBLE_PRIVILEGE))
                .Cast<VISIBLE_PRIVILEGE>()
                .Where(p => p != VISIBLE_PRIVILEGE.UNKNOWN)
                .Select(p => new VisiblePrivilegeDto
                {
                    Value = (uint)p,
                    Name = p.ToString(),
                    Description = p.GetDescription()
                })
                .ToList();

            var operatePrivileges = Enum.GetValues(typeof(OPERATE_PRIVILEGE))
                .Cast<OPERATE_PRIVILEGE>()
                .Where(p => p != OPERATE_PRIVILEGE.UNKNOWN)
                .Select(p => new OperatePrivilegeDto
                {
                    Value = (uint)p,
                    Name = p.ToString(),
                    Description = p.GetDescription()
                })
                .ToList();

            var privilegesDto = new PrivilegesDto
            {
                VisiblePrivileges = visiblePrivileges,
                OperatePrivileges = operatePrivileges
            };

            return Ok(privilegesDto);
        }

    }
}
