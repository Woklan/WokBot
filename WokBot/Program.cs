using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using WokBot.Interfaces;

namespace WokBot
{
    public class Program
    {
        private IServiceProvider _serviceProvider;

        public static void Main(string[] args) => new Program().MainAsync(args).GetAwaiter().GetResult();

        public async Task MainAsync(string[] args)
        {
            _serviceProvider = DependencyRegistry.RegisterDependencies();

            var bot = _serviceProvider.GetRequiredService<IBot>();

            await bot.StartBotAsync();
        }
    }
}
