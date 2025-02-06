using Discord.WebSocket;
using System.Threading.Tasks;

namespace WokBot.Interfaces
{
    public interface ICommandHandler
    {
        Task InstallCommandsAsync();
        Task HandleCommandAsync(SocketMessage messageParam);
    }
}
