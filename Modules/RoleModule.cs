using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Rosalyn.Data.Models;
using Rosalyn.Misc;
using Rosalyn.Preconditions;
using Rosalyn.Services;

namespace Rosalyn.Modules
{
    [Name("Special Roles Module"), Group("role")]
    [Summary("A module to manage a server's special roles")]
    public class RoleModule : ModuleBase<SocketCommandContext>
    {
        private readonly RoleService _roleService;
        private readonly ModerationLogService _moderationLogService;
        public RoleModule(RoleService service, ModerationLogService moderationLogService)
        {
            _roleService = service;
            _moderationLogService = moderationLogService;
        }

        [Command("set")]
        [Summary("Sets a role to become a special role")]
        [RequireCustomPermission("manage.roles")]
        public async Task SetSpecialRole(string type, IRole role)
        {
            await _roleService.SetSpecialRole(Context.Guild, role, type);
            await ReplyAsync($"This server's {type} role has been set to {role.Name}");
        }

        [Command("persist")]
        [Summary("Creates or removes a role persist on a user")]
        [Remarks("This command will overwrite an already existent role persist in the database unless no " +
                 "duration is provided, in which case it will either add a permanent role persist on the user " +
                 "or remove the role persist from the user (i.e. toggle)")]
        [RequireCustomPermission("roles.persist")]
        public async Task ToggleRolePersist(IGuildUser user, IRole role, TimeSpan? duration=null, [Remainder] string reason=null)
        {
            RolePersist rolePersist = await _roleService.GetRolePersist(Context.Guild, role, user);
            if (rolePersist == null)
            {
                // There is no role persist so make a new one
                await _roleService.CreateRolePersist(Context.Guild, role, user, duration);
                await ReplyAsync($"The {role.Name} role will now persist on {user.Username}#{user.Discriminator} " +
                                 (duration.HasValue ? $" for {duration.Value.ToHumanReadableString()}" : "indefinitely"));
            }
            else if (duration.HasValue)
            {
                // A role persist already exists and a duration value was supplied, so update the role persist
                rolePersist.Timestamp = DateTime.Now;
                rolePersist.Duration = duration.Value;
                await _roleService.UpdateRolePersist(rolePersist);
                await ReplyAsync($"The role persist for {user.Username}#{user.Discriminator} was updated");
            }
            else
            {
                // A role already exists and a duration value was not supplied so remove it
                await _roleService.RemoveRolePersist(rolePersist);
                await ReplyAsync($"Removed the {role.Name} role persist from {user.Username}#{user.Discriminator}");
            }
        }
    }
}