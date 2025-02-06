using Discord.Commands;
using System.Threading.Tasks;
using WokBot.Interfaces;

namespace WokBot.Services.Commands
{
    public class PingPongCommand : ModuleBase<SocketCommandContext>
    {
        private const string Pong = "Pong!";

        [Command("ping")]
        [Summary("Pongs the ping")]
        public async Task SayAsync()
            => await Context.Channel.SendMessageAsync(Pong);
    }
}
