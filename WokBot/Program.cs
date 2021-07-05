using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using WokBot.Classes;
using WokBot.Interfaces;

namespace WokBot
{
    public class Program
    {
        public static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();
        private DiscordSocketClient _client;
        private static CommandHandler commandHandler;
        public static CommandService Commands;
        public static ResourcesInterface resourcesInterface;
        public static Utility utility;
        public static ulong bot_id = 466905552648142848;
        public async Task MainAsync()
        {
            var _config = new DiscordSocketConfig { MessageCacheSize = 100 };
            _client = new DiscordSocketClient(_config);

            Commands = new CommandService();
            commandHandler = new CommandHandler(_client, Commands);

            resourcesInterface = JsonConvert.DeserializeObject<ResourcesInterface>(File.ReadAllText(@"C:\Users\Wokla\source\repos\WokBot\WokBot\resources.json").Replace('\"', ' '));

            utility = new Utility();

            await _client.LoginAsync(TokenType.Bot, resourcesInterface.discord);
            await commandHandler.InstallCommandsAsync();
            await _client.StartAsync();

            _client.MessageUpdated += MessageUpdated;
            _client.Ready += () =>
            {
                Console.WriteLine("Bot is connected!");
                return Task.CompletedTask;
            };

            await _client.SetGameAsync("Gaming", "https://twitch.tv/minemanluke", ActivityType.Streaming);

            await Task.Delay(-1);
        }
        private async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
        {
            var message = await before.GetOrDownloadAsync();
            Console.WriteLine($"{message} -> {after}");
        }
    }
}
