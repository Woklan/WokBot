using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using WokBot.Classes;
using Serilog.Events;
using Serilog;
using WokBot.Models.Config;
using System.Text.Json;
using System.IO;
using WokBot.Services.Commands;
using WokBot.Services;

namespace WokBot
{
    public class Program
    {
        private DiscordSocketClient _client;
        private IServiceProvider _serviceProvider;

        private CommandService _commandService;
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

            SetupCommands();

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
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            _client.Log += LogAsync;
        }

        private void SetupCommands()
        {
            _commandService = new CommandService();

            _commandHandler = new CommandHandler(_client, _commandService, _serviceProvider);
            _commandHandler.InstallCommandsAsync();
        }

        private async Task LogAsync(LogMessage message)
        {
            var severity = message.Severity switch
            {
                LogSeverity.Critical => LogEventLevel.Fatal,
                LogSeverity.Error => LogEventLevel.Error,
                LogSeverity.Warning => LogEventLevel.Warning,
                LogSeverity.Info => LogEventLevel.Information,
                LogSeverity.Verbose => LogEventLevel.Verbose,
                LogSeverity.Debug => LogEventLevel.Debug,
                _ => LogEventLevel.Information
            };
            
            Log.Write(severity, message.Exception, "[{Source}] {Message}", message.Source, message.Message);
            
            await Task.CompletedTask;
        }
    }
}
