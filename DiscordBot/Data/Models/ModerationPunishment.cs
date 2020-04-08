namespace DiscordBot.Data.Models
{
    public class ModerationPunishment
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public int Tier { get; set; }
        public string Punishment { get; set; }
    }
}