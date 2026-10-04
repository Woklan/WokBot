using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.Commands;
using System.Threading.Tasks;
using WokBot.Models.Config;

namespace WokBot
{
    public class Program
    {
        public static void Main(string[] args) => new Program().MainAsync(args).GetAwaiter().GetResult();

        public async Task MainAsync(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            builder.Services
                .AddDiscordGateway(options => options.Intents = GatewayIntents.All)
                .AddCommands()
                .AddHttpClient()
                .AddOptions()
                .Configure<UrbanDictionaryCommandConfiguration>(builder.Configuration.GetSection(nameof(UrbanDictionaryCommandConfiguration)));

            var host = builder.Build();

            host.AddModules(typeof(Program).Assembly);

            await host.RunAsync();
        }
    }
}
