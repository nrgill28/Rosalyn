using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Rosalyn.Data;
using Rosalyn.Services;

namespace Rosalyn
{
    class Program
    {
        public const string ApplicationVersion = "0.0.1";
        
        static void Main(string[] args)
        {
            Console.Title = "Rosalyn Discord Moderation Bot";
            new Program().Start().GetAwaiter().GetResult();
        }

        private ILogger _logger;
        private DiscordSocketClient _client;
        
        private async Task Start()
        {
            // Create the services collection
            using (ServiceProvider services = ConfigureServices())
            {
                // Get the config
                IConfiguration config = services.GetRequiredService<IConfiguration>();
                
                // Get the logger
                _logger = services.GetRequiredService<ILogger>();
                
                // Get the database
                DatabaseContext dbContext = services.GetRequiredService<DatabaseContext>();
                await dbContext.Database.EnsureCreatedAsync();
                
                // Get the command service
                CommandService commandService = services.GetRequiredService<CommandService>();
                commandService.Log += LogClientMessage;
                
                // Start the client
                DiscordSocketClient client = services.GetRequiredService<DiscordSocketClient>();
                _client = client;
                client.Log += LogClientMessage;
                client.Ready += OnReady;
                await client.LoginAsync(TokenType.Bot, config["token"]);
                await client.StartAsync();
                
                // Hook the command handler
                await services.GetRequiredService<CommandHandler>().InstallCommandsAsync();
                
                // Hang so the program doesn't exit
                await Task.Delay(-1);
            }
        }

        private async Task OnReady()
        {
            SocketSelfUser user = _client.CurrentUser;
            _logger.LogInformation($"Logged in as {user.Username}#{user.Discriminator}");
            await Task.CompletedTask;
        }

        private async Task LogClientMessage(LogMessage message)
        {
            switch (message.Severity)
            {
                case LogSeverity.Critical: _logger.LogCritical(message.Exception, message.Message); break;
                case LogSeverity.Error: _logger.LogError(message.Exception, message.Message); break;
                case LogSeverity.Warning: _logger.LogWarning(message.Exception, message.Message); break;
                case LogSeverity.Info: _logger.LogInformation(message.Exception, message.Message); break;
                default: _logger.LogDebug(message.Exception, message.Message); break;
            }

            await Task.CompletedTask;
        }
        
        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<DatabaseContext>()
                .AddSingleton<ModerationLogService>()
                .AddSingleton<BlacklistService>()
                .AddSingleton<PermissionsService>()
                .AddSingleton<RoleService>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .AddScoped(_ => ConfigureConfiguration())
                .AddScoped(_ => ConfigureLogging())
                .BuildServiceProvider();
        }

        private IConfiguration ConfigureConfiguration()
        {
            return new ConfigurationBuilder().AddJsonFile("config.json").Build();
        }

        private ILogger ConfigureLogging()
        {
            var factory = LoggerFactory.Create(builder => { builder.AddConsole(); });
            return factory.CreateLogger<Program>();
        }
    }
}