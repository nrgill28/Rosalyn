# Rosalyn
![.NET Core](https://github.com/nrgill28/Rosalyn/workflows/.NET%20Core/badge.svg)

Welcome to Rosalyn, my custom Discord server moderation bot.
Focused entirely around moderator ease of use, this bot provides multiple functions that aren't found in other moderation bots.

## Notable features
 - Role persists: users will keep persistent roles even if they leave and rejoin the server so they can't just rejoin to be unmuted. Additionally, you can have role persists set on a timer.
 - Automatic punishment tiering: Server administrators can define punishment rules according to categories and users will incrementally receive the appropriate punishment for having broken a rule in that category enough times.

## Todo
 - [ ] Moderation
   - [x] Blacklist
   - [ ] Warn, Ban, Kick
   - [x] Role persist
   - [ ] Mute (alias for muted role persist)
   - [ ] Slowmode
 - [x] Punishment tier system
 - [ ] Informational commands
   - [ ] Whois (user info)
   - [ ] Info (server info)
   - [ ] Status (bot into)
   - [x] Help
 - [ ] Server event logging
   - [ ] Guild setting for event log channel
 - [x] Server settings
 - [x] Permission management
