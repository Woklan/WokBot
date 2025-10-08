using Discord.Commands;
using System;
using System.Threading.Tasks;
using WokBot.Interfaces;

namespace WokBot.Services
{
    public class CommandServiceWrapper : ICommandServiceWrapper
    {
        private CommandService _commandService;

        public CommandServiceWrapper()
        {
            _commandService = new CommandService();
        }

        public async Task ExecuteAsync(ICommandContext context, int argPos, IServiceProvider services, MultiMatchHandling multiMatchingHandling = MultiMatchHandling.Exception)
                => await _commandService.ExecuteAsync(context, argPos, services, multiMatchingHandling);

        public async Task AddModuleAsync(Type type, IServiceProvider serviceProvider)
            => await _commandService.AddModuleAsync(type, serviceProvider);
    }
}
