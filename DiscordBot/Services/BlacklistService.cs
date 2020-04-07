using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Data;
using DiscordBot.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscordBot.Services
{
    public class BlacklistService
    {
        private readonly DatabaseContext _dbContext;
        private readonly PermissionsService _permissions;
        private readonly DiscordSocketClient _discord;

        public BlacklistService(DiscordSocketClient discord, DatabaseContext dbContext, PermissionsService permissions)
        {
            _dbContext = dbContext;
            _permissions = permissions;
            _discord = discord;

            // Bind to the events
            discord.MessageReceived += CheckMessageOnReceive;
            discord.MessageUpdated += CheckMessageOnUpdate;
        }

        private async Task CheckMessage(SocketMessage message)
        {
            // If the message wasn't sent in a server, or the user has permission to ignore the blacklist or is the bot, return
            if (!(message.Channel is SocketTextChannel textChannel)) return;
            if (message.Author.Id == _discord.CurrentUser.Id) return;
            if (await _permissions.UserHasPermission(message.Author, textChannel.Guild, "blacklist.ignore"))
                return;

            foreach (BlacklistFilter filter in _dbContext.BlacklistFilters)
                if (filter.Compiled.IsMatch(message.Content))
                    await message.DeleteAsync();
        }

        private async Task CheckMessageOnReceive(SocketMessage message) => await CheckMessage(message);

        private async Task CheckMessageOnUpdate(Cacheable<IMessage, ulong> before, SocketMessage after,
            ISocketMessageChannel channel)
            => await CheckMessage(after);

        /// <summary>
        /// Adds and returns a new blacklist filter
        /// </summary>
        /// <param name="regex">The Regex pattern to use</param>
        /// <param name="guild">The guild to use</param>
        /// <returns>The newly added filter</returns>
        public async Task<BlacklistFilter> AddBlacklistFilter(string regex, IGuild guild)
        {
            var entry = await _dbContext.AddAsync(new BlacklistFilter {Content = regex, ServerId = guild.Id});
            await _dbContext.SaveChangesAsync();
            return entry.Entity;
        }

        /// <summary>
        /// Removes a filter from the blacklist
        /// </summary>
        /// <param name="id">The ID of the filter to remove</param>
        /// <param name="guild">The guild</param>
        /// <returns>The removed filter</returns>
        public async Task<BlacklistFilter> RemoveBlacklistFilter(int id, IGuild guild)
        {
            var filter = await _dbContext.BlacklistFilters.FirstOrDefaultAsync(x => x.Id == id);
            if (filter.ServerId != guild.Id) return null;
            _dbContext.BlacklistFilters.Remove(filter);
            await _dbContext.SaveChangesAsync();
            return filter;
        }

        /// <summary>
        /// Returns an array of blacklist filters in the given server
        /// </summary>
        /// <param name="guild">The server to return filters from</param>
        /// <returns>The list of filters in the server</returns>
        public async Task<BlacklistFilter[]> ListBlacklistFilters(IGuild guild)
        {
            return await Task.Run(() =>
                _dbContext.BlacklistFilters.Where(x => x.ServerId == guild.Id).ToArray());
        }
    }
}