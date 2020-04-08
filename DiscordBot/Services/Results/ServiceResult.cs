using System;
using Discord.Commands;

namespace DiscordBot.Services.Results
{
    public class ServiceResult
    {
        public Exception Exception { get; }
        public bool Success { get; }
        public string Message { get; }

        public ServiceResult(bool success=true, string message=null, Exception exception = null)
        {
            Exception = exception;
            Success = success;
            Message = message;
        }
    }
}