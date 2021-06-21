﻿using System;
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
        public async Task MainAsync()
        {
            var _config = new DiscordSocketConfig { MessageCacheSize = 100 };
            _client = new DiscordSocketClient(_config);

            Commands = new CommandService();
            commandHandler = new CommandHandler(_client, Commands);

            utility = new Utility();

            resourcesInterface = JsonConvert.DeserializeObject<ResourcesInterface>(File.ReadAllText(@"C:\Users\Wokla\source\repos\WokBot\WokBot\resources.json").Replace('\"', ' '));
            

            await _client.LoginAsync(TokenType.Bot, resourcesInterface.discord);
            await commandHandler.InstallCommandsAsync();
            await _client.StartAsync();

            _client.MessageUpdated += MessageUpdated;
            _client.Ready += () =>
            {
                Console.WriteLine("Bot is connected!");
                return Task.CompletedTask;
            };

            await Task.Delay(-1);
        }
        private async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
        {
            var message = await before.GetOrDownloadAsync();
            Console.WriteLine($"{message} -> {after}");
        }
    }
}
