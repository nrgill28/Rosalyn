using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using DiscordBot.Data;
using DiscordBot.Data.Models;
using DiscordBot.Misc;
using DiscordBot.Preconditions;
using DiscordBot.Services;

namespace DiscordBot.Modules
{
    [Name("Punishments Module"), Group("punishments")]
    [Summary("A module for managing the server's automatic punishment system")]
    [RequireCustomPermission("guild.manage_punishments")]
    public class PunishmentsModule : ModuleBase<SocketCommandContext>
    {
        private readonly PunishmentsService _punishments;
        public PunishmentsModule(PunishmentsService punishments) => _punishments = punishments;

        [Command("list")]
        [Summary("Lists all the punishments in the server or a specific category if given")]
        public async Task ListPunishments(string category = null)
        {
            if (category == null)
            {
                StringBuilder stringBuilder = new StringBuilder($"__**Punishment categories in {Context.Guild.Name}:**__\n");
                foreach (var c in await _punishments.GetPunishmentCategories(Context.Guild))
                {
                    stringBuilder.Append(c.Name + ": ");
                    stringBuilder.AppendJoin(", ", c.Punishments.Select(x =>
                    {
                        if (x != null)
                            return (x.Duration.HasValue ? x.Duration?.ToShortHumanReadableString() + " " : "") + x.Type;
                        return "None";
                    }));
                    stringBuilder.AppendLine();
                }

                await ReplyAsync(stringBuilder.ToString());
            }
            else
            {
                // TODO: This.
                await ReplyAsync("Not implemented yet.");
            }
        }

        [Command("set")]
        [Summary("Sets a punishment level in the server's punishment list")]
        public async Task AddPunishment(string category, int tier, string punishment, TimeSpan? duration=null)
        {
            // Try and get the punishment type enum from the string
            if (Enum.TryParse(punishment, true, out PunishmentType type) &&
                Enum.IsDefined(typeof(PunishmentType), type))
            {
                await _punishments.SetRawPunishment(Context.Guild, category, tier, type, duration);
                await ReplyAsync("Added punishment.");
            }
            else
                await ReplyAsync("Invalid punishment type.");
        }

        [Command("remove")]
        [Summary("Removes a punishment level in the server's punishment list")]
        public async Task RemovePunishment(string category, int tier)
        {
            await _punishments.RemoveRawPunishment(Context.Guild, category, tier);
            await ReplyAsync("Removed punishment.");
        }
    }
}