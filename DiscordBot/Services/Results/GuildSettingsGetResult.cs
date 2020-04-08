using System;

namespace DiscordBot.Services.Results
{
    public class GuildSettingsGetResult : ServiceResult
    {
        public object Result { get; }

        public GuildSettingsGetResult(object result, bool success, string message, Exception exception) : base(success,
            message, exception)
        {
            Result = result;
        }
    }
}