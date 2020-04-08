using System;

namespace DiscordBot.Misc
{
    /// <summary>
    /// Adds a summary to a guild setting
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class GuildSettingAttribute : Attribute
    {
        public string Summary { get; }
        public bool Protected { get; }

        public GuildSettingAttribute(string summary, bool @protected=false)
        {
            Summary = summary;
            Protected = @protected;
        }
    }
}