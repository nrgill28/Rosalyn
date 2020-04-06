namespace Rosalyn.Data
{
    public static class Strings
    {
        public static readonly string ConfigurationLocation = "config.json";
        
        #region bot response messages
        public static readonly string WarnMessage = "You were warned in {guild} with the following reason: {reason}";
        public static readonly string TimedMuteMessage = "You were muted in {guild} for {duration} with the following reason: {reason}";
        public static readonly string UntimedMuteMessage = "You were muted in {guild} with the following reason: {reason}";
        public static readonly string TimedBanMessage = "You were banned in {guild} for {duration} with the following reason: {reason}";
        public static readonly string UntimedBanMessage = "You were permanently banned in {guild} with the following reason: {reason}";
        #endregion

    }
}