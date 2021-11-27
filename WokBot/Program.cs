using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WokBot.Classes;
using WokBot.Classes.Logging;
using WokBot.Interfaces;

namespace WokBot
{
    public class Program
    {
        public static void Main(string[] args) => new Program().MainAsync(args).GetAwaiter().GetResult();
        private DiscordSocketClient _client;
        private static CommandHandler commandHandler;
        public static CommandService Commands;
        public static ResourcesInterface resourcesInterface;
        public static Utility utility;
        public static ulong bot_id = 466905552648142848;
        public static ILogger logger;
        private static int logCount = 1;

        public async Task MainAsync(string[] args)
        {
            // Starts the Logger
            using IHost host = CreateHostBuilder(args).Build();
            logger = host.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation(generateLogNum(), "Logger Started...");

            DiscordSocketConfig _config = new DiscordSocketConfig { MessageCacheSize = 100 };
            _client = new DiscordSocketClient(_config);

            Commands = new CommandService();
            commandHandler = new CommandHandler(_client, Commands);

            if (System.Environment.GetEnvironmentVariable("docker") == null)
            {
                resourcesInterface = JsonConvert.DeserializeObject<ResourcesInterface>(File.ReadAllText(@"../../../resources.json").Replace('\"', ' '));
            }
            else
            {
                string json = JsonConvert.SerializeObject(System.Environment.GetEnvironmentVariables());
                resourcesInterface = JsonConvert.DeserializeObject<ResourcesInterface>(json);
            }

            utility = new Utility();

            await _client.LoginAsync(TokenType.Bot, resourcesInterface.discord);
            await commandHandler.InstallCommandsAsync();
            await _client.StartAsync();

            _client.Ready += () =>
            {
                Console.WriteLine("Bot is connected!");
                return Task.CompletedTask;
            };

            await _client.SetGameAsync("Gaming", "https://twitch.tv/minemanluke", ActivityType.Streaming);

            await Task.Delay(-1);
        }

        public static int generateLogNum()
        {
            logCount++;
            return logCount - 1;
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureLogging(builder =>
                builder.ClearProviders()
                .AddLogger(configuration =>
                {
                    configuration.LogLevels.Add(LogLevel.Warning, ConsoleColor.Magenta);
                    configuration.LogLevels.Add(LogLevel.Error, ConsoleColor.Red);
                }));
    }
}
