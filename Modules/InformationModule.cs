﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Rosalyn.Misc;
using Rosalyn.Preconditions;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Rosalyn.Modules
{
    [Name("Information Module")]
    [Summary("A module for all informative commands")]
    public class InformationModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _commands;

        public InformationModule(CommandService commands)
        {
            _commands = commands;
        }

        [Command("help")]
        [Summary("Shows a help menu for all commands or detailed help about a command or module")]
        public async Task Help(
            [Summary("If provided, shows detailed help for a command or module matching this name"), Remainder]
            string target = null)
        {
            if (target == null) await HelpMenu();
            else
            {
                CommandInfo commandInfo = _commands.Commands.FirstOrDefault(x => x.FullCommandName() == target);
                if (commandInfo == null)
                {
                    ModuleInfo moduleInfo = _commands.Modules.FirstOrDefault(x => x.FullModuleName() == target);
                    if (moduleInfo == null)
                        await ReplyAsync($"No command or module found matching name {Format.Sanitize(target)}");
                    else
                        await HelpModule(moduleInfo);
                }
                else
                    await HelpCommand(commandInfo);
            }
        }

        /// <summary>
        /// Generates and returns an embedded message containing information about a command
        /// </summary>
        private async Task HelpCommand(CommandInfo command)
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.Title = $"Command: {command.FullCommandName()}";
            builder.Description = command.Summary;

            builder.AddField(field =>
            {
                field.Name = "Usage:";
                field.Value = command.FullCommandName() + " " +
                              String.Join(" ", command.Parameters.Select(
                                  x => x.IsOptional ? $"[{x.Name}]" : $"<{x.Name}>"));
                field.IsInline = false;
            });


            // If the command has parameters, add a field for it
            if (command.Parameters.Count > 0)
                builder.AddField(field =>
                {
                    field.Name = "Parameters";
                    List<string> parameters = new List<string>();
                    foreach (var parameter in command.Parameters)
                        parameters.Add(parameter.Name + (parameter.IsOptional ? " (Optional)" : "") + ": " +
                                       parameter.Summary);
                    field.Value = String.Join('\n', parameters);
                });

            // If the command has remarks, add a field for it
            if (command.Remarks != "")
                builder.AddField(field =>
                {
                    field.Name = "Remarks:";
                    field.Value = command.Remarks;
                });

            // If the command requires custom permissions, add a field for it
            RequireCustomPermissionAttribute[] requiredPermissions =
                command.Preconditions.OfType<RequireCustomPermissionAttribute>().ToArray();
            if (requiredPermissions.Length > 0)
                builder.AddField(field =>
                {
                    field.Name = "Required Permissions";
                    field.Value = String.Join('\n', requiredPermissions.Select(x => x.Permission));
                });

            // Build the embed and send
            await ReplyAsync(embed: builder.Build());
        }

        /// <summary>
        /// Generated and sends an embedded message containing information about a module
        /// </summary>
        private async Task HelpModule(ModuleInfo module)
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.Title = module.FullModuleName();
            builder.Description = module.Summary;

            builder.AddField(field =>
            {
                field.Name = "Commands:";
                field.Value = String.Join('\n', module.Commands.Select(x => x.FullCommandName() + ": " + x.Summary));
            });

            await ReplyAsync(embed: builder.Build());
        }

        /// <summary>
        /// Generates a help menu
        /// </summary>
        private async Task HelpMenu()
        {
            // TODO: This.
            await ReplyAsync("This has not been implemented yet.");
        }

        [Command("info")]
        [Summary("Shows information about this bot")]
        public async Task Info()
        {
            // TODO: This.
            await ReplyAsync("This has not been implemented yet.");
        }

        [Command("status")]
        [Summary("Shows the bot's status")]
        public async Task Status()
        {
            // TODO: This.
            await ReplyAsync("This has not been implemented yet.");
        }
    }
}