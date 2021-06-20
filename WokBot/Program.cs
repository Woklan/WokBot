using System.Threading.Tasks;
using Discord.WebSocket;

namespace WokBot
{
    public class Program
    {
        public static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();
        private DiscordSocketClient _client;
        public async Task MainAsync() {
            _client = new DiscordSocketClient();

            var token = "";

            await _client.LoginAsync(Discord.TokenType.Bot, token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }
    }
}
