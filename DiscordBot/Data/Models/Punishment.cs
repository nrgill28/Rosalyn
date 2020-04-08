using System;

namespace DiscordBot.Data.Models
{
    public class Punishment
    {
        public PunishmentType Type { get; set; }
        public TimeSpan? Duration { get; set; }
    }
}