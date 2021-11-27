using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Options;

namespace WokBot.Classes.Logging
{
    public sealed class LoggerProvider : ILoggerProvider
    {
        private readonly IDisposable _onChangeToken;
        private LoggerConfig _currentConfig;
        private readonly ConcurrentDictionary<string, Logger> _loggers = new();
        

        public LoggerProvider(IOptionsMonitor<LoggerConfig> config)
        {
            _currentConfig = config.CurrentValue;
            _onChangeToken = config.OnChange(updatedConfig => _currentConfig = updatedConfig);
        }

        public ILogger CreateLogger(string categoryName) =>
            _loggers.GetOrAdd(categoryName, name => new Logger(name, GetCurrentConfig));

        private LoggerConfig GetCurrentConfig() => _currentConfig;

        public void Dispose()
        {
            _loggers.Clear();
            _onChangeToken.Dispose();
        }
    }
}
