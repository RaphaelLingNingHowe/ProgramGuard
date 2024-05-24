using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using ProgramGuard.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace ProgramGuard.Data
{
    public class ApplicationDBContext : IdentityDbContext<User>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
        : base(options)
        {
        }

        public DbSet<FileList> FileLists { get; set; }
        public DbSet<ChangeLog> ChangeLogs { get; set; }
        public DbSet<ActionLog> ActionLogs{ get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
           
            base.OnModelCreating(builder);
        }
    }

}
