using System.Threading.Tasks;
using WokBot.Services.VideoDownloadService;

namespace WokBot.Interfaces
{
    public interface IVideoDownloadService
    {
        Task<VideoDownloadWrapper> DownloadVideosAudioAsync(string url);
    }
}
