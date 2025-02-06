using System.Threading.Tasks;
using YoutubeDLSharp;

namespace WokBot.Interfaces
{
    public interface IVideoDownloadService
    {
        Task<RunResult<string>> DownloadVideosAudio(string url);
        void CleanVideoDownload(string fileDirectory);
    }
}
