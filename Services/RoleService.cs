using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Rosalyn.Data;
using Rosalyn.Data.Models;

namespace Rosalyn.Services
{
    public class RoleService
    {
        private readonly DatabaseContext _dbContext;
        private readonly DiscordSocketClient _discord;
        public RoleService(DatabaseContext dbContext, DiscordSocketClient discord)
        {
            _dbContext = dbContext;
            _discord = discord;
            _discord.UserJoined += OnUserJoin;
            _discord.Ready += OnReady;

        }

        #region Public Service Methods
        /// <summary>
        /// Sets a server's special role to the provided role
        /// </summary>
        /// <param name="guild">The guild to operate in</param>
        /// <param name="role">The role to become special</param>
        /// <param name="type">The type of special role</param>
        /// <returns>The newly inserted SpecialRole entity</returns>
        public async Task<SpecialRole> SetSpecialRole(IGuild guild, IRole role, string type)
        {
            SpecialRole entry = await _dbContext.SpecialRoles.FirstOrDefaultAsync(x 
                => x.GuildId == guild.Id && x.Name == type);
            if (entry == null)
            {
                var result = await _dbContext.AddAsync(new SpecialRole {GuildId = guild.Id, RoleId = role.Id, Name = type});
                entry = result.Entity;
            }
            else
            {
                entry.RoleId = role.Id;
                _dbContext.SpecialRoles.Update(entry);
            }

            await _dbContext.SaveChangesAsync();
            return entry;
        }

        /// <summary>
        /// Gets a server's special role from the provided type
        /// </summary>
        /// <param name="guild">The guild to get the role from</param>
        /// <param name="type">The type of role to get</param>
        /// <returns>The SpecialRole entity from the database</returns>
        public async Task<SpecialRole> GetSpecialRole(IGuild guild, string type)
            => await _dbContext.SpecialRoles.FirstOrDefaultAsync(x => x.GuildId == guild.Id && x.Name == type);

        /// <summary>
        /// Creates a new RolePersist entity in the database
        /// </summary>
        /// <param name="guild">The guild of the role persist</param>
        /// <param name="role">The role that is persistent</param>
        /// <param name="user">The user that it applies to</param>
        /// <param name="duration">The duration this role persist applies for</param>
        /// <returns>The newly created RolePersist entity</returns>
        public async Task<RolePersist> CreateRolePersist(IGuild guild, IRole role, IGuildUser user, TimeSpan? duration)
        {
            // Add the role to the user
            await user.AddRoleAsync(role);
            
            // Add the role persist to the database
            var result = await _dbContext.RolePersists.AddAsync(new RolePersist
            {
                Timestamp = DateTime.Now,
                GuildId = guild.Id,
                RoleId = role.Id,
                UserId = user.Id,
                Duration = duration
            });
            await _dbContext.SaveChangesAsync();

            StartTaskForRolePersist(result.Entity);
            
            return result.Entity;
        }

        /// <summary>
        /// Returns the role persist entity for a given guild, role and user
        /// </summary>
        /// <param name="guild">The guild to get the role persist from</param>
        /// <param name="role">The role for the role persist</param>
        /// <param name="user">The user for the role persist</param>
        /// <returns>The RolePersist entity or null if not found</returns>
        public async Task<RolePersist> GetRolePersist(IGuild guild, IRole role, IUser user)
        {
            return await _dbContext.RolePersists.FirstOrDefaultAsync(x =>
                x.GuildId == guild.Id && x.RoleId == role.Id && x.UserId == user.Id);
        }

        /// <summary>
        /// Updates a RolePersist entity in the database
        /// </summary>
        /// <param name="rolePersist">The updated entity</param>
        /// <returns>The Updated entity</returns>
        public async Task<RolePersist> UpdateRolePersist(RolePersist rolePersist)
        {
            var result = _dbContext.RolePersists.Update(rolePersist);
            await _dbContext.SaveChangesAsync();
            return result.Entity;
        }
        
        /// <summary>
        /// Removes a RolePersist entity from the database
        /// </summary>
        /// <param name="rolePersist">The RolePersist entity to remove</param>
        public async Task RemoveRolePersist(RolePersist rolePersist)
        {
            // Remove the role from the user if they have it
            SocketGuild guild = _discord.GetGuild(rolePersist.GuildId);
            SocketGuildUser user = guild.GetUser(rolePersist.UserId);
            SocketRole role = guild.GetRole(rolePersist.RoleId);
            await user.RemoveRoleAsync(role);
            
            // Remove if from the database
            _dbContext.RolePersists.Remove(rolePersist);
            await _dbContext.SaveChangesAsync();
        }
        
        /// <summary>
        /// Gets a user's role persists from the specified server
        /// </summary>
        /// <param name="guild">The guild to get role persists in</param>
        /// <param name="user">The user to get role persists from</param>
        /// <returns>The role persists</returns>
        public async Task<RolePersist[]> GetUserRolePersists(IGuild guild, IUser user)
            => _dbContext.RolePersists.Where(x => x.GuildId == guild.Id && x.UserId == user.Id).ToArray();
        #endregion

        #region Private Service Methods

        /// <summary>
        /// Starts a timer to remove a role persist when it is no longer active
        /// </summary>
        /// <param name="rolePersist">The role persist</param>
        private void StartTaskForRolePersist(RolePersist rolePersist)
        {
            // If it's indefinite don't bother starting a task
            if (!rolePersist.Duration.HasValue) return;
            
            // Find out how long it should go for
            TimeSpan timeout = rolePersist.Duration.Value - (DateTime.Now - rolePersist.Timestamp);
            if (timeout < TimeSpan.Zero) timeout = TimeSpan.Zero;
            
            // Create a new Task and delay the appropriate time
            Task.Delay(timeout).ContinueWith(t =>
            {
                // Fetch the RolePersist entity from the database again in case it changed or was removed
                RolePersist rp = _dbContext.RolePersists.Find(rolePersist.Id);
                
                // If it was removed, return
                if (rp == null) return;
                
                // Otherwise continue with removing it
                SocketGuild guild = _discord.GetGuild(rp.GuildId);
                SocketRole role = guild.GetRole(rp.RoleId);
                SocketGuildUser user = guild.GetUser(rp.UserId);
                user.RemoveRoleAsync(role).GetAwaiter().GetResult();
                RemoveRolePersist(rp).GetAwaiter().GetResult();
            });
        }
        #endregion
        
        #region Discord Event Handlers
        /// <summary>
        /// When the Discord client is ready, go through all the role persists in the database and make sure they're all applied
        /// </summary>
        private async Task OnReady()
        {
            // Go through all the role persists in the database and make sure they're all applied
            foreach (RolePersist rp in _dbContext.RolePersists)
            {
                if (!rp.Active) _dbContext.RolePersists.Remove(rp);
                else
                {
                    SocketGuild guild = _discord.GetGuild(rp.GuildId);
                    SocketRole role = guild.GetRole(rp.RoleId);
                    SocketGuildUser user = guild.GetUser(rp.UserId);

                    await user.AddRoleAsync(role);
                    StartTaskForRolePersist(rp);
                }
            }

            await _dbContext.SaveChangesAsync();
        }
        
        /// <summary>
        /// When a user joins a server, check if they have an active role persist and if they do, reapply it
        /// </summary>
        private async Task OnUserJoin(SocketGuildUser user)
        {
            // Get a list of all the role persists on the user in this guild
            RolePersist[] rolePersists = await GetUserRolePersists(user.Guild, user);
            
            // Add all the active role persists back
            await user.AddRolesAsync(rolePersists.Where(x => x.Active)
                .Select(x => user.Guild.GetRole(x.RoleId)));
        }
        #endregion
    }
}