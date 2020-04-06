using System.Threading.Tasks;
using Discord.Commands;
using Rosalyn.Data.Models;
using Rosalyn.Preconditions;
using Rosalyn.Services;

namespace Rosalyn.Modules
{
    [Name("Blacklist Module"), Group("blacklist"), Alias("bl")]
    [Summary("Module for managing the bot's blacklist filters")]
    [RequireCustomPermission("blacklist.manage")]
    public class BlacklistModule : ModuleBase<SocketCommandContext>
    {
        private readonly BlacklistService _service;
        public BlacklistModule(BlacklistService service) => _service = service;

        [Command("add")]
        [Summary("Adds a regex pattern to the blacklist")]
        public async Task Add([Summary("The regex pattern (in quotes)")] string pattern)
        {
            BlacklistFilter result = await _service.AddBlacklistFilter(pattern, Context.Guild.Id);
            await ReplyAsync($"Added `{result.Content}` (ID: {result.Id}) to the blacklist");
        }

        [Command("remove")]
        [Summary("Removes a pattern from the blacklist")]
        public async Task Remove([Summary("The ID of the pattern to remove")] int id)
        {
            BlacklistFilter result = await _service.RemoveBlacklistFilter(id, Context.Guild.Id);
            if (result != null)
                await ReplyAsync($"Removed `{result.Content}` (ID: {result.Id})");
            else await ReplyAsync($"Could not remove filter with ID {id}");
        }
    }
}