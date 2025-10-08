using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace WokBot.Interfaces
{
    public interface ICommandServiceWrapper
    {
        Task ExecuteAsync(ICommandContext context, int argPos, IServiceProvider services, MultiMatchHandling multiMatchingHandling = MultiMatchHandling.Exception);
        Task AddModuleAsync(Type type, IServiceProvider serviceProvider);
    }
}
