using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.Commands;
using System.Threading.Tasks;
using WokBot.Interfaces;
using WokBot.Services;
using WokBot.Services.VideoDownloadService;
using WokBotModels.Config;
using WokBotDatabase;

namespace WokBot
{
    public class Program
    {
        public static void Main(string[] args) => new Program().MainAsync(args).GetAwaiter().GetResult();

        public async Task MainAsync(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            builder.Services
                .AddDiscordGateway(options => options.Intents = GatewayIntents.All )
                .AddCommands()
                .AddHttpClient()
                .AddOptions()
                .AddDatabaseServices()
                .AddSingleton<IFfmpegService, FfmpegService>()
                .AddSingleton<IVideoDownloadService, VideoDownloadService>()
                .Configure<UrbanDictionaryCommandConfiguration>(builder.Configuration.GetSection(nameof(UrbanDictionaryCommandConfiguration)))
                .Configure<VideoDownloadServiceConfiguration>(builder.Configuration.GetSection(nameof(VideoDownloadServiceConfiguration)));

            var host = builder.Build();

            host.AddModules(typeof(Program).Assembly);

            await host.RunAsync();
        }
    }
}
