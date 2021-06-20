using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace WokBot.Commands
{
    public class PingPongModule : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        [Summary("Pongs the ping")]
        public async Task SayAsync()
        {
            await Context.Channel.SendMessageAsync("Pong!");
        }
    }
}
