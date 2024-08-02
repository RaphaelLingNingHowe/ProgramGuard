using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProgramGuard.Enums;
using ProgramGuard.Models;
namespace ProgramGuard.Data
{
    public class ApplicationDBContext : IdentityDbContext<AppUser>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
        : base(options)
        {
        }
        public DbSet<FileList> FileLists { get; set; }
        public DbSet<ChangeLog> ChangeLogs { get; set; }
        public DbSet<ActionLog> ActionLogs { get; set; }
        public DbSet<PasswordHistory> PasswordHistories { get; set; }
        public DbSet<PrivilegeRule> PrivilegeRules { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //builder.Entity<AppUser>()
            //.Property(u => u.LockoutEnd)
            //.HasConversion(
            //    v => v.HasValue ? v.Value.LocalDateTime : (DateTime?)null,
            //    v => v.HasValue ? new DateTimeOffset(v.Value, TimeZoneInfo.Local.GetUtcOffset(v.Value)) : (DateTimeOffset?)null
            //);

            List<PrivilegeRule> privilegeRules = new List<PrivilegeRule>
            {
                new PrivilegeRule {Id = 1, Name = "管理員" , Visible = (VISIBLE_PRIVILEGE)31, Operate = (OPERATE_PRIVILEGE)2047 },
                new PrivilegeRule {Id = 2, Name = "審核員" , Visible = (VISIBLE_PRIVILEGE)23, Operate = (OPERATE_PRIVILEGE)10 },
                new PrivilegeRule {Id = 3, Name = "用戶" , Visible = (VISIBLE_PRIVILEGE)3, Operate = (OPERATE_PRIVILEGE)3 }
            };
            builder.Entity<PrivilegeRule>().HasData(privilegeRules);
        }
    }
}
