using System;
using DiscordBot.Data.Models;

namespace DiscordBot.Services.Results
{
    public class GuildSettingsUpdateResult : ServiceResult
    {
        public GuildSettings Result { get; }

        public GuildSettingsUpdateResult(GuildSettings result, bool success, string message, Exception exception)
        : base(success, message, exception)
        {
            Result = result;
        }
    }
}