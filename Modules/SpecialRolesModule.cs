using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Rosalyn.Services;

namespace Rosalyn.Modules
{
    [Name("Special Roles Module"), Group("roles")]
    [Summary("A module to manage a server's special roles")]
    public class SpecialRolesModule : ModuleBase<SocketCommandContext>
    {
        private readonly RoleService _service;

        public SpecialRolesModule(RoleService service) => _service = service;
        
        [Command("set")]
        [Summary("Sets a role to become a special role")]
        public async Task SetSpecialRole(IRole role, string type)
        {
            
        }
    }
}