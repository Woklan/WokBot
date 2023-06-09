using System.IO;
using System.Threading.Tasks;
using YoutubeDLSharp;

namespace WokBot.Services
{
    public class VideoDownloadService
    {
        private const string YoutubeDlBinaryPath = "yt-dlp.exe";
        private const string FfmpegBinaryPath = "ffmpeg.exe";

        public VideoDownloadService()
        {}

        public async Task<RunResult<string>> DownloadVideosAudio(string url)
        {
            var downloader = new YoutubeDL()
            {
                YoutubeDLPath = YoutubeDlBinaryPath,
                FFmpegPath = FfmpegBinaryPath,
            };

            var result = await downloader.RunAudioDownload(url);


            return result;
        }

        public void CleanVideoDownload(string fileDirectory)
            => File.Delete(fileDirectory);
    }
}
