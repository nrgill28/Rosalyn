using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using DiscordBot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot.Preconditions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireCustomPermissionAttribute : PreconditionAttribute
    {
        /// <summary>
        /// The permission name required to run the command
        /// </summary>
        private readonly string _permissionName;

        public string Permission => _permissionName;
        
        public RequireCustomPermissionAttribute(string permissionName)
        {
            _permissionName = permissionName;
        }
        
        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (context.Guild== null) return PreconditionResult.FromError("This command cannot be ran outside a guild.");
            
            IConfiguration config = services.GetRequiredService<IConfiguration>();
            PermissionsService permissions = services.GetRequiredService<PermissionsService>();
            
            
            // If the user has the requested permission, or overall admin, or is owner of the bot, return success
            if (await permissions.UserHasPermission(context.User, context.Guild, _permissionName) ||
                await permissions.UserHasPermission(context.User, context.Guild, "*") ||
                context.User.Id == ulong.Parse(config["owner_id"]))
                return PreconditionResult.FromSuccess();
            
            // Get a list of all parent permission namespaces
            string[] namespaces = _permissionName.Split(".");
            for (int i = namespaces.Length - 1; i > 0; i--)
            {
                // For each parent namespace suffixed with .*
                string check = String.Join('.', namespaces.Take(i)) + ".*";
                
                // If they have that permission return success
                if (await permissions.UserHasPermission(context.User, context.Guild, check)) return PreconditionResult.FromSuccess();
            }
            
            // Otherwise return error
            return PreconditionResult.FromError("You are not allowed to run this command");
        }
    }
}