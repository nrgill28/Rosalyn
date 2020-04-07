namespace DiscordBot.Data.Models
{
    public class SpecialRole
    {
        public int Id { get; set; }
        public ulong GuildId { get; set; }
        public ulong RoleId { get; set; }
        public string Name { get; set; }
    }
}