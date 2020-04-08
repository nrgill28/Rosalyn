using System;
using System.Collections.Generic;
using System.Linq;
using Discord.Commands;

namespace DiscordBot.Misc
{
    public static class Extensions
    {
        public static string ToHumanReadableString(this TimeSpan timeSpan)
        {
            List<string> pieces = new List<string>();
            if (timeSpan.Days >= 1d)
                pieces.Add($"{timeSpan.Days} days");
            if (timeSpan.Hours >= 1d)
                pieces.Add($"{timeSpan.Hours} hours");
            if (timeSpan.Minutes >= 1d)
                pieces.Add($"{timeSpan.Minutes} minutes");
            if (timeSpan.Seconds >= 1d)
                pieces.Add($"{timeSpan.Seconds} seconds");
            if (timeSpan.Milliseconds >= 1d)
                pieces.Add($"{timeSpan.Milliseconds} milliseconds");
            return String.Join(", ", pieces);
        }

        public static string FullCommandName(this CommandInfo commandInfo)
        {
            List<string> pieces = new List<string> {commandInfo.Name};
            ModuleInfo parent = commandInfo.Module;
            while (parent != null)
            {
                if (!String.IsNullOrEmpty(parent.Group))
                    pieces.Add(parent.Group);
                parent = parent.Parent;
            }

            pieces.Reverse();
            return String.Join(" ", pieces);
        }

        public static string Usage(this CommandInfo commandInfo)
        {
            return commandInfo.FullCommandName() + " " +
                string.Join(" ", commandInfo.Parameters.Select(
                    x => x.IsOptional ? $"[{x.Name}]" : $"<{x.Name}>"));
        }

        public static string FullModuleName(this ModuleInfo moduleInfo)
        {
            List<string> pieces = new List<string> {moduleInfo.Group};
            ModuleInfo parent = moduleInfo.Parent;
            while (parent != null)
            {
                if (!String.IsNullOrEmpty(parent.Group))
                    pieces.Add(parent.Group);
                parent = parent.Parent;
            }

            pieces.Reverse();
            return String.Join(" ", pieces);
        }
    }
}