using System;
using System.IO;
using System.Threading.Tasks;
using Discord.WebSocket;
using Newtonsoft.Json;
using WokBot.Interfaces;

namespace WokBot
{
    public class Program
    {
        public static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();
        private DiscordSocketClient _client;
        public async Task MainAsync() {
            _client = new DiscordSocketClient();

            ResourcesInterface result = JsonConvert.DeserializeObject<ResourcesInterface>(File.ReadAllText(@"C:\Users\Wokla\source\repos\WokBot\WokBot\resources.json").Replace('\"', ' '));

            Console.WriteLine("LOGIN");

            await _client.LoginAsync(Discord.TokenType.Bot, result.discord);
            await _client.StartAsync();

            await Task.Delay(-1);
        }
    }
}
