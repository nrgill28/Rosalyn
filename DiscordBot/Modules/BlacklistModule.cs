using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBot.Data.Models;
using DiscordBot.Preconditions;
using DiscordBot.Services;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace DiscordBot.Modules
{
    [Name("Blacklist Module"), Group("blacklist")]
    [Summary("Module for managing the bot's blacklist filters")]
    [RequireCustomPermission("blacklist.manage")]
    public class BlacklistModule : ModuleBase<SocketCommandContext>
    {
        private readonly BlacklistService _service;
        public BlacklistModule(BlacklistService service) => _service = service;

        [Command("add")]
        [Summary("Adds a regex pattern to the blacklist")]
        public async Task Add(
            [Summary("The regex pattern"), Remainder]
            string pattern)
        {
            var result = await _service.AddBlacklistFilter(pattern, Context.Guild);
            await ReplyAsync($"Added {Format.Sanitize(result.Content)} (ID: {result.Id}) to the blacklist");
        }

        [Command("remove")]
        [Summary("Removes a pattern from the blacklist")]
        public async Task Remove(
            [Summary("The ID of the pattern to remove")]
            int id)
        {
            var result = await _service.RemoveBlacklistFilter(id, Context.Guild);
            if (result != null)
                await ReplyAsync($"Removed {Format.Sanitize(result.Content)} (ID: {result.Id})");
            else await ReplyAsync($"Could not remove filter with ID {id}");
        }

        [Command("list")]
        [Summary("Lists the patterns in the blacklist")]
        public async Task List()
        {
            BlacklistFilter[] filters = await _service.ListBlacklistFilters(Context.Guild);
            if (filters.Length == 0)
                await ReplyAsync("There are no blacklist filters set");
            else
                await ReplyAsync("Filters:\n" +
                                 String.Join('\n', filters.Select(
                                     x => $"{x.Id}: {Format.Sanitize(x.Content)}")));
        }
    }
}