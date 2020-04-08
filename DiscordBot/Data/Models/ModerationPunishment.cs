using System;

namespace DiscordBot.Data.Models
{
    public class ModerationPunishment
    {
        public int Id { get; set; }
        public ulong GuildId { get; set; }
        public string Category { get; set; }
        public int Tier { get; set; }
        public PunishmentType Type { get; set; }
        public TimeSpan? Duration { get; set; }
    }
}