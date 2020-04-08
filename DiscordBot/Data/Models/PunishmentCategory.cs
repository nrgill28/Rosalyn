using System.Collections.Generic;

namespace DiscordBot.Data.Models
{
    public class PunishmentCategory
    {
        public string Name { get; set; }
        public List<Punishment> Punishments { get; set; }
    }
}