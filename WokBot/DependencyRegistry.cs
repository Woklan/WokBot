using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using WokBot.Services.Commands;
using WokBot.Services;
using Discord;

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
            AddCommandSingletons(serviceCollection);

            return serviceCollection.BuildServiceProvider();
        }

        private static IServiceCollection AddServiceSingletons(IServiceCollection serviceCollection)
           => serviceCollection
           .AddSingleton<VideoDownloadService>()
           .AddSingleton<FfmpegService>();

        private static IServiceCollection AddCommandSingletons(IServiceCollection serviceCollection)
            => serviceCollection
            .AddSingleton<PingPong>()
            .AddSingleton<UrbanDictionary>()
            .AddSingleton<Youtube>();
    }
}
