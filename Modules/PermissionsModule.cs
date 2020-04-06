﻿using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Rosalyn.Data.Models;
using Rosalyn.Preconditions;
using Rosalyn.Services;

namespace Rosalyn.Modules
{
    [Name("Permissions Module"), Group("permissions")]
    [Summary("Module for managing permissions")]
    [RequireCustomPermission("permissions.manage")]
    public class PermissionsModule : ModuleBase<SocketCommandContext>
    {
        private readonly PermissionsService _service;
        public PermissionsModule(PermissionsService service) => _service = service;

        [Command("give")]
        [Summary("Gives permissions to a user")]
        public async Task GiveUserPermissions(IUser user, [Remainder] string permissions)
        {
            string[] split = permissions.Split();
            foreach (string permission in split)
                await _service.GiveUserPermission(user, Context.Guild, permission);
            await ReplyAsync($"Gave {split.Length} permission(s) to {user.Username}#{user.Discriminator}");
        }
        
        [Command("take")]
        [Summary("Takes (revokes) permissions from a user")]
        public async Task RevokeUserPermissions(IUser user, [Remainder] string permissions)
        {
            string[] split = permissions.Split();
            foreach (string permission in split)
                await _service.RevokeUserPermission(user, Context.Guild, permission);
            await ReplyAsync($"Took {split.Length} permission(s) from {user.Username}#{user.Discriminator}");
        }

        [Command("list")]
        [Summary("Shows a list of what permissions a user has")]
        public async Task ShowUserPermissions(IUser user)
        {
            var permissions = await _service.GetUserPermissions(user, Context.Guild);
            StringBuilder sb = new StringBuilder($"Permissions for {user.Username}#{user.Discriminator}:\n```");
            sb.Append(String.Join('\n', permissions.Select(x => x.Permission)));
            sb.Append("```");
            await ReplyAsync(sb.ToString());
        }
    }
}