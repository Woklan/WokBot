using Microsoft.Extensions.Logging;
using System;

namespace WokBot.Classes.Logging
{
    class Logger : ILogger
    {
        private readonly string _name;
        private readonly Func<LoggerConfig> _getCurrentConfig;
       

        public Logger(
            string name,
            Func<LoggerConfig> getCurrentConfig) =>
            (_name, _getCurrentConfig) = (name, getCurrentConfig);

        public IDisposable BeginScope<TState>(TState state) => default!;

        public bool IsEnabled(LogLevel logLevel) =>
            _getCurrentConfig().LogLevels.ContainsKey(logLevel);

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventID,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            LoggerConfig config = _getCurrentConfig();

            if(config.EventID == 0 || config.EventID == eventID.Id)
            {
                ConsoleColor originalColor = Console.ForegroundColor;

                Console.ForegroundColor = config.LogLevels[logLevel];
                Console.WriteLine($"[{eventID.Id,2}: {logLevel,-12}]");

                Console.ForegroundColor = originalColor;
                Console.WriteLine($"    {_name} - {formatter(state, exception)}");
            }
        }
    }
}
