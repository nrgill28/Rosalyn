using System;

namespace Rosalyn.Data.Models
{
    public class ModerationLogEvent
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        
        public ulong ModeratorId { get; set; }
        public ulong TargetId { get; set; }
        public ulong ServerId { get; set; }
        
        public string Action { get; set; }
        public TimeSpan? Duration { get; set; }
        public string Reason { get; set; }

        public DateTime FinishedAt => Timestamp + Duration ?? DateTime.MaxValue;
    }
}