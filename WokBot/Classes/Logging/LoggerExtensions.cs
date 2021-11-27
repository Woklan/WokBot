using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using System;

namespace WokBot.Classes.Logging
{
    public static class LoggerExtensions
    {
        public static ILoggingBuilder AddLogger(this ILoggingBuilder builder)
        {
            builder.AddConfiguration();

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, LoggerProvider>());

            LoggerProviderOptions.RegisterProviderOptions<LoggerConfig, LoggerProvider>(builder.Services);

            return builder;
        }

        public static ILoggingBuilder AddLogger(this ILoggingBuilder builder, Action<LoggerConfig> configure)
        {
            builder.AddLogger();
            builder.Services.Configure(configure);

            return builder;
        }
    }
}
