using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using Rosalyn.Data;
using Rosalyn.Services;

namespace Rosalyn.Preconditions
{
    public class RequirePermissionAttribute : PreconditionAttribute
    {
        private readonly string _permissionName;
        
        public RequirePermissionAttribute(string permissionName)
        {
            _permissionName = permissionName;
        }
        
        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            DatabaseContext dbContext = services.GetRequiredService<DatabaseContext>();
            PermissionsService permissions = services.GetRequiredService<PermissionsService>();

            if (await permissions.UserHasPermission(context.User, context.Guild, _permissionName) ||
                await permissions.UserHasPermission(context.User, context.Guild, "*"))
                return PreconditionResult.FromSuccess();
            
            string[] namespaces = _permissionName.Split(".");
            for (int i = namespaces.Length - 1; i > 0; i--)
            {
                string check = String.Join('.', namespaces.Take(i)) + ".*";
                if (await permissions.UserHasPermission(context.User, context.Guild, check)) return PreconditionResult.FromSuccess();
            }
            
            return PreconditionResult.FromError("You are not allowed to run this command");
        }
    }
}