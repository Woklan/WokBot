using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private static CommandHandler _commandHandler;
        public static CommandService Commands;
        public static ResourcesInterface resourcesInterface;
        public static Utility utility;
        public static ulong bot_id = 466905552648142848;
        public static ILogger logger;
        private static int _logCount = 1;

        public async Task MainAsync(string[] args)
        {
            // Starts the Logger
            using IHost host = CreateHostBuilder(args).Build();
            logger = host.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation(generateLogNum(), "Logger Started...");

            DiscordSocketConfig _config = new DiscordSocketConfig { MessageCacheSize = 100 };
            _client = new DiscordSocketClient(_config);

            Commands = new CommandService();
            _commandHandler = new CommandHandler(_client, Commands);

            if (Environment.GetEnvironmentVariable("docker") == null)
            {
                resourcesInterface = JsonConvert.DeserializeObject<ResourcesInterface>(File.ReadAllText(@"../../../resources.json").Replace('\"', ' '));
            }
            else
            {
                resourcesInterface = JsonConvert.DeserializeObject<ResourcesInterface>(JsonConvert.SerializeObject(Environment.GetEnvironmentVariables()));
            }

            utility = new Utility();

            await _client.LoginAsync(TokenType.Bot, resourcesInterface.discord);
            await _commandHandler.InstallCommandsAsync();
            await _client.StartAsync();

            _client.MessageReceived += MessageReceived;

            _client.Ready += () =>
            {
                logger.LogInformation(generateLogNum(), "Bot is Connected!");
                return Task.CompletedTask;
            };

            await _client.SetGameAsync("Gaming", "https://twitch.tv/minemanluke", ActivityType.Streaming);

            await Task.Delay(-1);
        }

        private async Task MessageReceived(SocketMessage message)
        {
            if (message.Author.Id == _client.CurrentUser.Id) return;
            
            var attachments = message.Attachments.Where(x => Path.GetExtension(x.Url) == ".zip");

            if (attachments.Count() == 0) return;

            var videoOutput = resourcesInterface.video_output;
            var zipInsight = new ZipInsight();
            var listString = "";

            foreach (var attachment in attachments)
            {
                var url = attachment.Url;

                var list = zipInsight.ZipContents(url, videoOutput);

                foreach (var entry in list) listString += entry + "\n";

                var embedBuilder = new EmbedBuilder()
                {
                    Title = "Contents of: " + attachment.Filename,
                    Description = listString
                };

                await message.Channel.SendMessageAsync(embed: embedBuilder.Build());
            }
        }

        public static int generateLogNum()
        {
            _logCount++;
            return _logCount - 1;
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureLogging(builder =>
                builder.ClearProviders()
                .AddLogger(configuration =>
                {
                    configuration.LogLevels.Add(LogLevel.Debug, ConsoleColor.Blue);
                    configuration.LogLevels.Add(LogLevel.Warning, ConsoleColor.Magenta);
                    configuration.LogLevels.Add(LogLevel.Error, ConsoleColor.Red);
                }));
    }
}
