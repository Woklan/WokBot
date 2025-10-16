using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WokBot.Classes;
using WokBot.Interfaces;
using WokBot.Models.Config;

namespace WokBot.Services
{
    public class Bot : IBot
    {
        private DiscordSocketClient _client;
        private readonly ILogger<Bot> _logger;
        private readonly IServiceProvider _serviceProvider;
        private BotConfiguration _configuration;

        public Bot(DiscordSocketClient client, ILogger<Bot> logger, IServiceProvider serviceProvider, IOptions<BotConfiguration> configuration)
        {
            _client = client;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _configuration = configuration.Value;
        }

        public async Task StartBotAsync()
        {
            var config = await GetConfigAsync();

            await SetupCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, config.discord);

            await _client.StartAsync();

            _client.Log += Log;

            _client.Ready += () =>
            {
                return Task.CompletedTask;
            };

            await _client.SetGameAsync(_configuration.GameActivity, _configuration.ActivityLink, ActivityType.Streaming);

            await Task.Delay(Timeout.Infinite);
        }

        private async Task SetupCommandsAsync()
        {
            var commandService = _serviceProvider.GetRequiredService<ICommandServiceWrapper>();
            var configuration = _serviceProvider.GetRequiredService<IOptions<CommandHandlerConfiguration>>();

            var commandHandler = new CommandHandler(_client, commandService, _serviceProvider, configuration);
            await commandHandler.InstallCommandsAsync();
        }

        private async Task<BotConfig> GetConfigAsync()
        {
            var isRunningDocker = Environment.GetEnvironmentVariable(_configuration.DockerIndicator) != null;

            var json = isRunningDocker
                ? JsonSerializer.Serialize(Environment.GetEnvironmentVariables())
                : await File.ReadAllTextAsync(_configuration.DefaultSecretLocation);

            // TODO: DO NOT SERIALIZE TO DESERIALIZE FOR DOCKER
            return JsonSerializer.Deserialize<BotConfig>(json);
        }

        private Task Log(LogMessage message)
        {
            var messageToBeLogged = $"{message.Message}";

            var severity = message.Severity switch
            {
                LogSeverity.Critical => LogLevel.Error,
                LogSeverity.Error => LogLevel.Error,
                LogSeverity.Warning => LogLevel.Warning,
                LogSeverity.Info => LogLevel.Information,
                LogSeverity.Verbose => LogLevel.Debug,
                LogSeverity.Debug => LogLevel.Debug,
                _ => LogLevel.Information
            };

            _logger.Log(severity, messageToBeLogged);

            return Task.CompletedTask;
        }
    }
}
