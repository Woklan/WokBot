using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using WokBot.Interfaces;
using WokBot.Models.Config;
using WokBot.Services;

namespace WokBot
{
    public static class DependencyRegistry
    {
        private const int MessageCacheSize = 100;
        private const GatewayIntents BotGatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent;

        private const string _appsettingsName = "appsettings.json";
            
        public static IServiceProvider RegisterDependencies()
        {
            var config = new DiscordSocketConfig()
            {
                MessageCacheSize = MessageCacheSize,
                GatewayIntents = BotGatewayIntents
            };

            var serviceCollection = new ServiceCollection()
                .AddSingleton(config)
                .AddSingleton<DiscordSocketClient>()
                .AddHttpClient()
                .AddLogging(builder =>
                {
                    builder.AddSimpleConsole(options => {
                        options.SingleLine = true;
                    });

                    builder.AddDebug();
                });

            AddServiceSingletons(serviceCollection);
            AddConfiguration(serviceCollection);

            return serviceCollection.BuildServiceProvider();
        }

        private static IServiceCollection AddServiceSingletons(IServiceCollection serviceCollection)
           => serviceCollection
           .AddSingleton<IVideoDownloadService, VideoDownloadService>()
           .AddSingleton<IFfmpegService, FfmpegService>()
           .AddSingleton<ICommandServiceWrapper, CommandServiceWrapper>();

        private static IServiceCollection AddConfiguration(IServiceCollection serviceCollection)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(_appsettingsName, optional: true, reloadOnChange: true)
                .Build();

            return serviceCollection
                .AddOptions()
                .Configure<UrbanDictionaryCommandConfiguration>(configuration.GetSection(nameof(UrbanDictionaryCommandConfiguration)))
                .Configure<PingPongCommandConfiguration>(configuration.GetSection(nameof(PingPongCommandConfiguration)))
                .Configure<UrbanDictionaryCommandConfiguration>(configuration.GetSection(nameof(UrbanDictionaryCommandConfiguration)))
                .Configure<VideoDownloadServiceConfiguration>(configuration.GetSection(nameof(VideoDownloadServiceConfiguration)))
                .Configure<CommandHandlerConfiguration>(configuration.GetSection(nameof(CommandHandlerConfiguration)));
        }
    }
}
