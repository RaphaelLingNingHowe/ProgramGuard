using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgramGuard.Base;
using ProgramGuard.Data;
using ProgramGuard.Dtos.PrivilegeRule;
using ProgramGuard.Enums;
using ProgramGuard.Models;

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
            if (VisiblePrivilege.HasFlag(VISIBLE_PRIVILEGE.SHOW_ACCOUNT_PRIVILEGE) == false)
            {
                return Forbidden("沒有權限");
            }
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
                return ServerError(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePrivilegeRuleAsync(CreatePrivilegeDto createPrivilegeDto)
        {
            if (OperatePrivilege.HasFlag(OPERATE_PRIVILEGE.MODIFY_PRIVILEGE_RULE) == false)
            {
                return Forbidden("沒有權限");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest("驗證失敗，請檢查輸入的格式");
            }

            try
            {
                if (await _context.PrivilegeRules.AnyAsync(p => p.Name == createPrivilegeDto.Name))
                {
                    return Conflict("權限名稱已被使用");
                }

                var existingPrivilegeRule = await _context.PrivilegeRules.SingleOrDefaultAsync(o => o.Visible == createPrivilegeDto.Visible && o.Operate == createPrivilegeDto.Operate);

                if (existingPrivilegeRule != null)
                {
                    return Conflict("權限規則已存在");
                }

                var privilege = new PrivilegeRule()
                {
                    Name = createPrivilegeDto.Name,
                    Visible = createPrivilegeDto.Visible,
                    Operate = createPrivilegeDto.Operate
                };

                await _context.PrivilegeRules.AddAsync(privilege);
                await _context.SaveChangesAsync();
                await LogActionAsync(ACTION.ADD_PRIVILEGE_RULE, $"權限Id{privilege.Id}, 權限名稱{privilege.Name}");
                return Created(privilege);
            }
            catch (Exception ex)
            {
                return ServerError(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePrivilegeRuleAsync(int id, UpdatePrivilegeDto dto)
        {
            if (OperatePrivilege.HasFlag(OPERATE_PRIVILEGE.MODIFY_PRIVILEGE_RULE) == false)
            {
                return Forbidden("沒有權限");
            }
            var privilegeRule = await _context.PrivilegeRules.FindAsync(id);
            if (privilegeRule == null)
            {
                return NotFound($"找不到此權限規則[{id}]");
            }
            var existingPrivilegeRule = await _context.PrivilegeRules.SingleOrDefaultAsync(o => o.Visible == dto.Visible && o.Operate == dto.Operate);
            if (existingPrivilegeRule != null)
            {
                return Conflict($"已具有相同規則-{existingPrivilegeRule.Name}");
            }
            privilegeRule.Visible = dto.Visible;
            privilegeRule.Operate = dto.Operate;
            await _context.SaveChangesAsync();
            await LogActionAsync(ACTION.MODIFY_PRIVILEGE_RULE, $"權限規則已調整[{privilegeRule.Name}]");
            return Ok("權限規則內容更新成功");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            if (OperatePrivilege.HasFlag(OPERATE_PRIVILEGE.MODIFY_PRIVILEGE_RULE) == false)
            {
                return Forbidden("沒有權限");
            }
            try
            {
                var privilegeRule = await _context.PrivilegeRules.FindAsync(id);
                if (privilegeRule == null)
                {
                    return NotFound($"找不到此權限規則[{id}]");
                }

                if (await _context.Users.AnyAsync(o => o.Privilege == id))
                {
                    return Forbidden($"此規則已被帳號引用、無法刪除");
                }

                _context.PrivilegeRules.Remove(privilegeRule);

                await _context.SaveChangesAsync();
                await LogActionAsync(ACTION.MODIFY_PRIVILEGE_RULE, $"權限規則: {privilegeRule.Name}");
                return NoContent();
            }
            catch (Exception ex)
            {
                return ServerError(ex);
            }
        }

        [HttpGet("/PrivilegeRule")]
        public async Task<IActionResult> GetPrivilegeRulesAsync()
        {
            var privilegesDto = await Task.Run(() =>
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

                return new PrivilegesDto
                {
                    VisiblePrivileges = visiblePrivileges,
                    OperatePrivileges = operatePrivileges
                };
            });

            return Ok(privilegesDto);
        }
    }
}
