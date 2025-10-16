using Discord.WebSocket;
using System.Threading.Tasks;

namespace WokBot.Interfaces
{
    public interface IBot
    {
        Task StartBotAsync();
    }
}
