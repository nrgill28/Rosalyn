using System;

namespace DiscordBot.Data.Models
{
    public class RolePersist
    {
        public int Id { get; set; }
        public ulong GuildId { get; set; }
        public ulong UserId { get; set; }
        public ulong RoleId { get; set; }
        public DateTime Timestamp { get; set; }
        public TimeSpan? Duration { get; set; }

        public bool Active => !Duration.HasValue || DateTime.Now > Timestamp + Duration;
    }
}