using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.EntityFrameworkCore;
using Rosalyn.Data.Models;

namespace Rosalyn.Data
{
    public class DatabaseContext : DbContext
    {
        public DbSet<ModerationLogEvent> ModerationLogEvents { get; set; }
        
        public DbSet<BlacklistFilter> BlacklistFilters { get; set; }
        
        public DbSet<PermissionEntry> Permissions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={ApplicationEnvironment.ApplicationBasePath}/Rosalyn.db");
        }
    }
}