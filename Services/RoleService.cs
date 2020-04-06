using System.Threading.Tasks;
using Discord;
using Microsoft.EntityFrameworkCore;
using Rosalyn.Data;
using Rosalyn.Data.Models;

namespace Rosalyn.Services
{
    public class RoleService
    {
        private readonly DatabaseContext _dbContext;
        public RoleService(DatabaseContext dbContext) => _dbContext = dbContext;

        /// <summary>
        /// Sets a server's special role to the provided role
        /// </summary>
        /// <param name="guild">The guild to operate in</param>
        /// <param name="role">The role to become special</param>
        /// <param name="type">The type of special role</param>
        /// <returns></returns>
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
    }
}