using System;
using System.Threading.Tasks;
using Discord;
using Rosalyn.Data;
using Rosalyn.Data.Models;

namespace Rosalyn.Services
{
    public class ModerationLogService
    {
        private DatabaseContext _dbContext;

        public ModerationLogService(DatabaseContext dbContext) => _dbContext = dbContext;
        
        /// <summary>
        /// Adds an event to the moderation log
        /// </summary>
        /// <param name="moderatorId">The ID of the moderator performing the action</param>
        /// <param name="targetId">The ID of the user receiving the action</param>
        /// <param name="action">The type of action</param>
        /// <param name="duration">The duration of the action (or null if infinite)</param>
        /// <param name="reason">The reason for performing the action</param>
        /// <returns>The added ModerationLogEvent entity</returns>
        public async Task<ModerationLogEvent> AddEvent(ulong moderatorId, ulong targetId, ulong serverId, string action, TimeSpan? duration, string reason)
        {
            var entry = await _dbContext.ModerationLogEvents.AddAsync(new ModerationLogEvent
            {
                Timestamp = DateTime.Now,
                ModeratorId = moderatorId,
                TargetId = targetId,
                ServerId = serverId,
                Action = action,
                Duration = duration,
                Reason = reason
            });
            await _dbContext.SaveChangesAsync();
            
            return entry.Entity;
        }

        /// <summary>
        /// Adds a warn event to the moderation log
        /// </summary>
        /// <param name="moderatorId">The ID of the moderator performing the warn</param>
        /// <param name="targetId">The ID of the user receiving the warn</param>
        /// <param name="serverId">The ID of the server the warn was given in</param>
        /// <param name="reason">The reason for the warn</param>
        /// <returns>The added ModerationLogEvent entity</returns>
        public async Task<ModerationLogEvent> AddWarn(ulong moderatorId, ulong targetId, ulong serverId, string reason) =>
            await AddEvent(moderatorId, targetId, serverId, "warn", TimeSpan.Zero, reason);
        
        /// <summary>
        /// Adds a unmute event to the moderation log
        /// </summary>
        /// <param name="moderatorId">The ID of the moderator performing the unmute</param>
        /// <param name="targetId">The ID of the user receiving the unmute</param>
        /// <param name="serverId">The ID of the server the unmute was given in</param>
        /// <param name="reason">The reason for the unmute</param>
        /// <returns>The added ModerationLogEvent entity</returns>
        public async Task<ModerationLogEvent> AddUnmute(ulong moderatorId, ulong targetId, ulong serverId, string reason) =>
            await AddEvent(moderatorId, targetId, serverId, "unmute", TimeSpan.Zero, reason);
        
        /// <summary>
        /// Adds a unban event to the moderation log
        /// </summary>
        /// <param name="moderatorId">The ID of the moderator performing the unban</param>
        /// <param name="targetId">The ID of the user receiving the unban</param>
        /// <param name="serverId">The ID of the server the unban was given in</param>
        /// <param name="reason">The reason for the unban</param>
        /// <returns>The added ModerationLogEvent entity</returns>
        public async Task<ModerationLogEvent> AddUnban(ulong moderatorId, ulong targetId, ulong serverId, string reason) =>
            await AddEvent(moderatorId, targetId, serverId, "unban", TimeSpan.Zero, reason);

        /// <summary>
        /// Adds a mute event to the moderation log
        /// </summary>
        /// <param name="moderatorId">The ID of the moderator performing the mute</param>
        /// <param name="targetId">The ID of the user receiving the mute</param>
        /// <param name="serverId">The ID of the server the mute was given in</param>
        /// <param name="reason">The reason for the mute</param>
        /// <param name="duration">The duration of the mute (or null if infinite)</param>
        /// <returns>The added ModerationLogEvent entity</returns>
        public async Task<ModerationLogEvent> AddMute(ulong moderatorId, ulong targetId, ulong serverId, string reason, TimeSpan? duration=null) =>
            await AddEvent(moderatorId, targetId, serverId, "mute", duration, reason);

        /// <summary>
        /// Adds a ban event to the moderation log
        /// </summary>
        /// <param name="moderatorId">The ID of the moderator performing ban</param>
        /// <param name="targetId">The ID of the user receiving the ban</param>
        /// <param name="serverId">The ID of the server the ban was given in</param>
        /// <param name="reason">The reason for the ban</param>
        /// <param name="duration">The duration of the ban (or null if infinite)</param>
        /// <returns>The added ModerationLogEvent entity</returns>
        public async Task<ModerationLogEvent> AddBan(ulong moderatorId, ulong targetId, ulong serverId, string reason, TimeSpan? duration=null) =>
            await AddEvent(moderatorId, targetId, serverId, "ban", duration, reason);

        /// <summary>
        /// Adds a kick event to the moderation log
        /// </summary>
        /// <param name="moderatorId">The ID of the moderator performing the kick</param>
        /// <param name="targetId">The ID of the user receiving the kick</param>
        /// <param name="serverId">The ID of the server the kick was given in</param>
        /// <param name="reason">The reason for the kick</param>
        /// <returns>The added ModerationLogEvent entity</returns>
        public async Task<ModerationLogEvent> AddKick(ulong moderatorId, ulong targetId, ulong serverId, string reason) =>
            await AddEvent(moderatorId, targetId, serverId, "kick", TimeSpan.Zero, reason);
    }
}