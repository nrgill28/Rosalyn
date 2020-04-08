using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using DiscordBot.Data;
using DiscordBot.Data.Models;
using DiscordBot.Misc;
using DiscordBot.Services.Results;
using Microsoft.EntityFrameworkCore;

namespace DiscordBot.Services
{
    public class GuildService
    {
        private readonly DatabaseContext _dbContext;

        public GuildService(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<GuildSettings> GetGuildSettings(IGuild guild)
        {
            var result = await _dbContext.GuildSettings.FirstOrDefaultAsync(x => x.GuildId == guild.Id);
            if (result != null) return result;

            GuildSettings @default = GuildSettings.Defaults;
            @default.GuildId = guild.Id;
            
            result = (await _dbContext.AddAsync(@default)).Entity;
            await _dbContext.SaveChangesAsync();

            return result;
        }

        public async Task<GuildSettingsUpdateResult> SetGuildSetting(IGuild guild, string settingName, string settingValue)
        {
            // Get the current settings
            var settings = await GetGuildSettings(guild);

            // Try getting the property with the provided name
            PropertyInfo prop = settings.GetType().GetProperty(settingName);
            if (prop == null)
                return new GuildSettingsUpdateResult(settings, false, $"Guild setting {Format.Sanitize(settingName)} not found", null);

            // If it is protected, return
            var propertySettings = prop.GetCustomAttributes().OfType<GuildSettingAttribute>().FirstOrDefault();
            if (propertySettings != null && propertySettings.Protected)
                return new GuildSettingsUpdateResult(settings, false, "This setting is protected, you cannot change it.", null);
            
            // Try getting a TypeConverter for the property
            try
            {
                TypeConverter converter = TypeDescriptor.GetConverter(prop.PropertyType);
                prop.SetValue(settings, converter.ConvertFromString(settingValue));
            }
            catch (NotSupportedException e)
            {
                // This is thrown when the provided string value cannot be converted to the required type
                return new GuildSettingsUpdateResult(settings, false, $"Invalid value! Value for {Format.Sanitize(settingName)} needs to be convertible to {prop.PropertyType}", e);
            }
            
            // If we made it here is means we successfully changed the property
            _dbContext.GuildSettings.Update(settings);
            await _dbContext.SaveChangesAsync();
            return new GuildSettingsUpdateResult(settings, true, "", null);
        }

        public async Task<GuildSettingsGetResult> GetGuildSettingValueAsString(IGuild guild, string settingName)
        {
            // Get the current settings
            GuildSettings settings = await GetGuildSettings(guild);

            // Try and get the property, if it doesn't exist, return
            PropertyInfo property = settings.GetType().GetProperty(settingName);
            if (property == null)
                return new GuildSettingsGetResult(null, false, $"Guild setting {Format.Sanitize(settingName)} not found", null);

            // If it does exist, then return it's value
            return new GuildSettingsGetResult(property.GetValue(settings), true, "", null);
        }
    }
}