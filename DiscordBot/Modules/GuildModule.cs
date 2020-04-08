using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBot.Data.Models;
using DiscordBot.Misc;
using DiscordBot.Preconditions;
using DiscordBot.Services;

namespace DiscordBot.Modules
{
    [Name("Guild Module")]
    [Summary("A module for controlling the guild's settings")]
    public class GuildModule : ModuleBase<SocketCommandContext>
    {
        private readonly GuildService _guildService;

        public GuildModule(GuildService guildService)
        {
            _guildService = guildService;
        }
        
        [Command("settings")]
        [Summary("View a list of all settings, the value of a single setting, or update a setting")]
        [RequireCustomPermission("guild.manage")]
        public async Task Settings(
            [Summary("The setting to view or update")]string settingName=null,
            [Summary("The value to update the setting with"), Remainder]string settingValue=null)
        {
            // If the setting name and value are set, update the value
            if (!string.IsNullOrEmpty(settingName) && !string.IsNullOrEmpty(settingValue))
                await UpdateSetting(settingName, settingValue);
            
            // If only the name is set, show the value
            else if (!string.IsNullOrEmpty(settingName))
                await ShowSetting(settingName);
            
            // Otherwise show a list of all settings
            else
            {
                // Get the current settings
                GuildSettings settings = await _guildService.GetGuildSettings(Context.Guild);
                StringBuilder stringBuilder = new StringBuilder($"__**Settings for {Context.Guild.Name}:**__\n```");
                
                // For all the properties in the GuildSettings class...
                foreach (PropertyInfo property in settings.GetType().GetProperties())
                {
                    // Only show properties with the custom attribute
                    if (property.GetCustomAttributes().OfType<GuildSettingAttribute>().Any())
                        stringBuilder.AppendLine($"{property.Name}: {property.GetValue(settings)}");
                }

                // Reply
                stringBuilder.Append("```");
                await ReplyAsync(stringBuilder.ToString());
            }
        }

        private async Task UpdateSetting(string settingName, string settingValue)
        {
            var result = await _guildService.SetGuildSetting(Context.Guild, settingName, settingValue);
            if (!result.Success)
                await ReplyAsync(result.Message);
            else
                await ReplyAsync("Setting updated.");
        }

        private async Task ShowSetting(string settingName)
        {
            var value = await _guildService.GetGuildSettingValueAsString(Context.Guild, settingName);
            if (!value.Success)
                await ReplyAsync(value.Message);
            else
                await ReplyAsync($"{Format.Sanitize(settingName)}: {value.Result ?? "null"}");
        }
    }
}