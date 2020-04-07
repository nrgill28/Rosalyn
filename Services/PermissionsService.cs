using System.Linq;
using System.Threading.Tasks;
using Discord;
using Microsoft.EntityFrameworkCore;
using Rosalyn.Data;
using Rosalyn.Data.Models;

namespace Rosalyn.Services
{
    public class PermissionsService
    {
        private readonly DatabaseContext _dbContext;

        public PermissionsService(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Check whether a user has a permission in a guild.
        /// Also matches when a user has a wildcard permission for any parent-level permission
        /// e.g. blacklist.* will match blacklist.manage, blacklist.ignore ... etc.
        /// </summary>
        /// <param name="user">The user to check</param>
        /// <param name="guild">The guild to check in</param>
        /// <param name="permission">The permission to check for</param>
        /// <returns>True if the user has the permission, false otherwise</returns>
        public async Task<bool> UserHasPermission(IUser user, IGuild guild, string permission)
        {
            return await _dbContext.Permissions.AnyAsync(x
                => x.ForeignId == user.Id && x.ServerId == guild.Id && x.Permission == permission);
        }

        /// <summary>
        /// Gives a user a permission in a particular guild
        /// </summary>
        /// <param name="user">The user to give a permission to</param>
        /// <param name="guild">The guild to give the user permission in</param>
        /// <param name="permission">The permission to give</param>
        /// <returns>The newly added PermissionEntry entity</returns>
        public async Task<PermissionEntry> GiveUserPermission(IUser user, IGuild guild, string permission)
        {
            var result = await _dbContext.Permissions.AddAsync(new PermissionEntry
            {
                ServerId = guild.Id,
                ForeignId = user.Id,
                Permission = permission
            });

            await _dbContext.SaveChangesAsync();

            return result.Entity;
        }

        /// <summary>
        /// Takes away a user's permission in a particular guild
        /// </summary>
        /// <param name="user">The user to take a permission from</param>
        /// <param name="guild">The guild to take their permission from in</param>
        /// <param name="permission">The permission to take</param>
        /// <returns>The permission that was taken (or null if it didn't exist)</returns>
        public async Task<PermissionEntry> RevokeUserPermission(IUser user, IGuild guild, string permission)
        {
            PermissionEntry entity = await _dbContext.Permissions.FirstOrDefaultAsync(x
                => x.ForeignId == user.Id && x.ServerId == guild.Id && x.Permission == permission);
            _dbContext.Permissions.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Gets an array of all the permissions a user has in a particular guild
        /// </summary>
        /// <param name="user">The user to get permissions of</param>
        /// <param name="guild">Which guild to filter to</param>
        /// <returns>A list of permissions for that user in that guild</returns>
        public async Task<PermissionEntry[]> GetUserPermissions(IUser user, IGuild guild)
        {
            return await _dbContext.Permissions.Where(x
                => x.ForeignId == user.Id && x.ServerId == guild.Id).ToArrayAsync();
        }
    }
}