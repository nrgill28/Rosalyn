using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using Rosalyn.Data;
using Rosalyn.Services;

namespace Rosalyn.Preconditions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireCustomPermissionAttribute : PreconditionAttribute
    {
        /// <summary>
        /// The permission name required to run the command
        /// </summary>
        private readonly string _permissionName;
        
        public RequireCustomPermissionAttribute(string permissionName)
        {
            _permissionName = permissionName;
        }
        
        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            DatabaseContext dbContext = services.GetRequiredService<DatabaseContext>();
            PermissionsService permissions = services.GetRequiredService<PermissionsService>();

            // If the user has the requested permission, or overall admin, return success
            if (await permissions.UserHasPermission(context.User, context.Guild, _permissionName) ||
                await permissions.UserHasPermission(context.User, context.Guild, "*"))
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