using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WokBot.Classes;
using WokBot.Interfaces;
using WokBot.Models.Config;

namespace WokBot
{
    public class Program
    {
        private DiscordSocketClient _client;
        private IServiceProvider _serviceProvider;

        private CommandHandler _commandHandler;

        private const string Gaming = "Gaming";
        private const string MinemanlukeTwitchLink = "https://twitch.tv/minemanluke";
        private const string Docker = "docker";
        private const string DefaultConfigLocation = "config.json";
        public static void Main(string[] args) => new Program().MainAsync(args).GetAwaiter().GetResult();

        public async Task MainAsync(string[] args)
        {
            _serviceProvider = DependencyRegistry.RegisterDependencies();
            _client = _serviceProvider.GetRequiredService<DiscordSocketClient>();

            SetupLogging();

            await SetupCommandsAsync(_serviceProvider);

            var config = await GetConfigAsync();

            await StartBot(config);
        }

        private async Task<BotConfig> GetConfigAsync()
        {
            var isRunningDocker = Environment.GetEnvironmentVariable(Docker) != null;

            var json = isRunningDocker
                ? JsonSerializer.Serialize(Environment.GetEnvironmentVariables())
                : await File.ReadAllTextAsync(DefaultConfigLocation);

            // TODO: DO NOT SERIALIZE TO DESERIALIZE FOR DOCKER
            return JsonSerializer.Deserialize<BotConfig>(json);
        }

        private async Task StartBot(BotConfig config)
        {
            await _client.LoginAsync(TokenType.Bot, config.discord);

            await _client.StartAsync();

            _client.Ready += () =>
            {
                return Task.CompletedTask;
            };

            await _client.SetGameAsync(Gaming, MinemanlukeTwitchLink, ActivityType.Streaming);

            await Task.Delay(Timeout.Infinite);
        }

        private void SetupLogging()
        {
            _client.Log += Log;
        }

        private async Task SetupCommandsAsync(IServiceProvider  serviceProvider)
        {
            var commandService = _serviceProvider.GetRequiredService<ICommandServiceWrapper>();

            _commandHandler = new CommandHandler(_client, commandService, _serviceProvider);
            await _commandHandler.InstallCommandsAsync();
        }

        private Task Log(LogMessage message)
        {
            var logger = _serviceProvider.GetRequiredService<ILogger<Program>>();

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

            logger.Log(severity, messageToBeLogged);

            return Task.CompletedTask;
        }
    }
}
