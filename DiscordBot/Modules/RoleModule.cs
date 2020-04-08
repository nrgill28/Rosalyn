using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBot.Data.Models;
using DiscordBot.Misc;
using DiscordBot.Preconditions;
using DiscordBot.Services;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace DiscordBot.Modules
{
    [Name("Special Roles Module"), Group("role")]
    [Summary("A module to manage a server's special roles")]
    public class RoleModule : ModuleBase<SocketCommandContext>
    {
        private readonly RoleService _roleService;
        public RoleModule(RoleService service) => _roleService = service;

        [Command("persist")]
        [Summary("Creates or removes a role persist on a user")]
        [Remarks("This command will overwrite an already existent role persist in the database unless no " +
                 "duration is provided, in which case it will either add a permanent role persist on the user " +
                 "or remove the role persist from the user (i.e. toggle)")]
        [RequireCustomPermission("roles.persist")]
        public async Task ToggleRolePersist(
            [Summary("The user to modify role persists of")]
            IGuildUser user,
            [Summary("The role to modify")] IRole role,
            [Summary("The duration this role will persist for")]
            TimeSpan? duration = null)
        {
            RolePersist rolePersist = await _roleService.GetRolePersist(Context.Guild, role, user);
            if (rolePersist == null)
            {
                // There is no role persist so make a new one
                await _roleService.CreateRolePersist(Context.Guild, role, user, duration);
                await ReplyAsync($"The {role.Name} role will now persist on {user.Username}#{user.Discriminator} " +
                                 (duration.HasValue
                                     ? $" for {duration.Value.ToHumanReadableString()}"
                                     : "indefinitely"));
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