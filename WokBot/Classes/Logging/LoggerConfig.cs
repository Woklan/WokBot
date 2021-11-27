using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace WokBot.Classes.Logging
{
    public class LoggerConfig
    {
        public int EventID { get; set; }

        public Dictionary<LogLevel, ConsoleColor> LogLevels { get; set; } = new()
        {
            [LogLevel.Information] = ConsoleColor.Green,
        };
    }
}
