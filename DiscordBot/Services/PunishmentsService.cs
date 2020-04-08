using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using DiscordBot.Data;
using DiscordBot.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscordBot.Services
{
    public class PunishmentsService
    {
        private readonly DatabaseContext _dbContext;
        public PunishmentsService(DatabaseContext dbContext) => _dbContext = dbContext;

        public async Task<PunishmentCategory> GetPunishmentCategory(IGuild guild, string category)
        {
            IEnumerable<ModerationPunishment> punishments = await GetRawPunishmentsFromCategory(guild, category);
            PunishmentCategory punishmentCategory = new PunishmentCategory
            {
                Name = category,
                Punishments = new List<Punishment>()
            };
            foreach (var punishment in punishments)
            {
                
                // Make sure the list is large enough for it
                while (punishmentCategory.Punishments.Count <= punishment.Tier)
                    punishmentCategory.Punishments.Add(null);
                
                // Add it
                punishmentCategory.Punishments[punishment.Tier] = new Punishment
                {
                    Duration = punishment.Duration,
                    Type = punishment.Type
                };
            }

            return punishmentCategory;
        }

        public async Task<IEnumerable<ModerationPunishment>> GetRawPunishments(IGuild guild)
        {
            return await Task.Run(() =>
                _dbContext.ModerationPunishments.Where(x =>
                    x.GuildId == guild.Id));
        }

        public async Task<IEnumerable<ModerationPunishment>> GetRawPunishmentsFromCategory(IGuild guild, string category)
        {
            return await Task.Run(() =>
                _dbContext.ModerationPunishments.Where(x =>
                    x.GuildId == guild.Id && x.Category == category));
        }
        
        public async Task<IEnumerable<PunishmentCategory>> GetPunishmentCategories(IGuild guild)
        {
            IEnumerable<ModerationPunishment> punishments = await GetRawPunishments(guild);
            Dictionary<string, PunishmentCategory> categories = new Dictionary<string, PunishmentCategory>();
            foreach (var punishment in punishments)
            {
                // If the category doesn't exist yet, add it
                if (!categories.ContainsKey(punishment.Category))
                    categories[punishment.Category] = new PunishmentCategory
                    {
                        Name = punishment.Category,
                        Punishments = new List<Punishment>()
                    };
                
                // Make sure the list is large enough for it
                while (categories[punishment.Category].Punishments.Count <= punishment.Tier)
                    categories[punishment.Category].Punishments.Add(null);
                
                // Add it
                categories[punishment.Category].Punishments[punishment.Tier] = new Punishment
                {
                    Duration = punishment.Duration,
                    Type = punishment.Type
                };
            }

            return categories.Values;
        }

        public async Task<ModerationPunishment> GetRawPunishment(IGuild guild, string category, int tier)
        {
            return await _dbContext.ModerationPunishments.Where(x =>
                x.GuildId == guild.Id && x.Category == category && x.Tier == tier).FirstOrDefaultAsync();
            
        }
        
        public async Task<ModerationPunishment> SetRawPunishment(IGuild guild, string category, int tier, PunishmentType type, TimeSpan? duration)
        {
            var result = await GetRawPunishment(guild, category, tier);
            if (result == null)
            {
                result = (await _dbContext.ModerationPunishments.AddAsync(new ModerationPunishment
                {
                    GuildId = guild.Id,
                    Category = category,
                    Tier = tier,
                    Type = type,
                    Duration = duration
                })).Entity;
            }
            else
            {
                result.Type = type;
                result.Duration = duration;
                _dbContext.ModerationPunishments.Update(result);
            }
            await _dbContext.SaveChangesAsync();
            return result;
        }

        public async Task<ModerationPunishment> RemoveRawPunishment(IGuild guild, string category, int tier)
        {
            var result = await _dbContext.ModerationPunishments.Where(x =>
                x.GuildId == guild.Id && x.Category == category && x.Tier == tier).FirstOrDefaultAsync();
            _dbContext.Remove(result);
            await _dbContext.SaveChangesAsync();
            return result;
        }
    }
}