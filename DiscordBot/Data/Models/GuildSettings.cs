using System.ComponentModel.DataAnnotations;
using DiscordBot.Misc;

namespace DiscordBot.Data.Models
{
    public class GuildSettings
    {
        [Key]
        [GuildSetting("This is the ID for your guild. You cannot change this.", true)]
        public ulong GuildId { get; set; }
        
        [GuildSetting("Sets the command prefix for this guild")]
        [MaxLength(4)]
        public string CommandPrefix { get; set; }
        
        [GuildSetting("Sets whether or not the bot will respond to messages with the command prefix but the command is invalid")]
        public bool RespondOnInvalidCommand { get; set; }

        [GuildSetting("Sets the server's muted role ID")]
        public ulong? MutedRoleId { get; set; }
        
        public static GuildSettings Defaults => new GuildSettings
        {
            CommandPrefix = "^",
            RespondOnInvalidCommand = true
        };
    }
}