using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Rosalyn.Data;
using Rosalyn.Data.Models;

namespace Rosalyn.Services
{
    public class BlacklistService
    {
        private readonly DatabaseContext _dbContext;
        private readonly PermissionsService _permissions;

        public BlacklistService(DiscordSocketClient discord, DatabaseContext dbContext, PermissionsService permissions)
        {
            _dbContext = dbContext;
            _permissions = permissions;

            // Bind to the events
            discord.MessageReceived += CheckMessageOnReceive;
            discord.MessageUpdated += CheckMessageOnUpdate;
        }

        private async Task CheckMessage(SocketMessage message)
        {
            // If the message wasn't sent in a server, or the user has permission to ignore the blacklist return
            if (!(message.Channel is SocketTextChannel textChannel)) return;
            if (await _permissions.UserHasPermission(message.Author, textChannel.Guild, "blacklist.ignore"))
                return;
            
            foreach (BlacklistFilter filter in _dbContext.BlacklistFilters)
                if (filter.Compiled.IsMatch(message.Content)) await message.DeleteAsync();
        }

        private async Task CheckMessageOnReceive(SocketMessage message) => await CheckMessage(message);

        private async Task CheckMessageOnUpdate(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
            => await CheckMessage(after);

        /// <summary>
        /// Adds and returns a new blacklist filter
        /// </summary>
        /// <param name="regex">The Regex pattern to use</param>
        /// <param name="serverId">The ID of the server</param>
        /// <returns>The newly added filter</returns>
        public async Task<BlacklistFilter> AddBlacklistFilter(string regex, ulong serverId)
        {
            var entry = await _dbContext.AddAsync(new BlacklistFilter {Content = regex, ServerId = serverId});
            await _dbContext.SaveChangesAsync();
            return entry.Entity;
        }

        /// <summary>
        /// Removes a filter from the blacklist
        /// </summary>
        /// <param name="id">The ID of the filter to remove</param>
        /// <param name="serverId">The ID of the server</param>
        /// <returns>The removed filter</returns>
        public async Task<BlacklistFilter> RemoveBlacklistFilter(int id, ulong serverId)
        {
            var filter = await _dbContext.BlacklistFilters.FirstOrDefaultAsync(x => x.Id == id);
            if (filter.ServerId != serverId) return null;
            _dbContext.BlacklistFilters.Remove(filter);
            await _dbContext.SaveChangesAsync();
            return filter;
        }
    }
}