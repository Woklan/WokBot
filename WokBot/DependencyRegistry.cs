using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using WokBot.Services;
using Discord;
using WokBot.Interfaces;

namespace WokBot
{
    public static class DependencyRegistry
    {
        private const int MessageCacheSize = 100;
        private const GatewayIntents BotGatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent;

        public static IServiceProvider RegisterDependencies()
        {
            var config = new DiscordSocketConfig()
            {
                MessageCacheSize = MessageCacheSize,
                GatewayIntents = BotGatewayIntents
            };

            var serviceCollection = new ServiceCollection()
                .AddSingleton(config)
                .AddSingleton<DiscordSocketClient>();

            AddServiceSingletons(serviceCollection);

            return serviceCollection.BuildServiceProvider();
        }

        private static IServiceCollection AddServiceSingletons(IServiceCollection serviceCollection)
           => serviceCollection
           .AddSingleton<IVideoDownloadService, VideoDownloadService>()
           .AddSingleton<IFfmpegService, FfmpegService>()
           .AddSingleton<ICommandServiceWrapper, CommandServiceWrapper>();
    }
}
