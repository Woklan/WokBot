using Discord.Commands;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using WokBot.Models.Config;

namespace WokBot.Services.Commands
{
    public class PingPongCommand : ModuleBase<SocketCommandContext>
    {
        private readonly PingPongCommandConfiguration _configuration;

        public PingPongCommand(IOptions<PingPongCommandConfiguration> configuration)
        {
            _configuration = configuration.Value;
        }

        [Command("ping")]
        [Summary("Pongs the ping")]
        public async Task SayAsync()
            => await Context.Channel.SendMessageAsync(_configuration.OutputText);
    }
}
