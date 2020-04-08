using DiscordBot.Data.Models;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DiscordBot.Data
{
    public class DatabaseContext : DbContext
    {
        private readonly IConfiguration _config;

        public DatabaseContext(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// This constructor is for design-time initialization of the class
        /// </summary>
        public DatabaseContext()
        {
            _config = new ConfigurationBuilder().AddJsonFile("config.json").Build();
        }
        
        public DbSet<ModerationLogEvent> ModerationLogEvents { get; set; }
        public DbSet<BlacklistFilter> BlacklistFilters { get; set; }
        public DbSet<PermissionEntry> Permissions { get; set; }
        public DbSet<RolePersist> RolePersists { get; set; }
        public DbSet<GuildSettings> GuildSettings { get; set; }
        public DbSet<ModerationPunishment> ModerationPunishments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (_config["database:source"] == "sqlite")
                options.UseSqlite($"Data Source={ApplicationEnvironment.ApplicationBasePath}/{_config["path"]}");
            else if (_config["database:source"] == "mysql")
                options.UseMySql($"server={_config["database:host"]};database={_config["database:schema"]};user={_config["database:user"]};password={_config["database:pass"]}");
            
        }
    }
}