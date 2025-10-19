using NetCord.Services.Commands;
using System;

namespace WokBot.Services.Commands
{
    public class PingPongCommand : CommandModule<CommandContext>
    {
        public PingPongCommand()
        {

        }

        [Command("ping")]
        public string Ping()
            => $"Pong! {Math.Round(Context.Client.Latency.TotalMilliseconds)} ms";
    }
}
