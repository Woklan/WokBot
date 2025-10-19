using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;
using WokBot.Interfaces;
using WokBot.Models.Config;
using YoutubeDLSharp;

namespace WokBot.Services
{
    public class VideoDownloadService : IVideoDownloadService
    {
        private VideoDownloadServiceConfiguration _configuration;

        public VideoDownloadService(IOptions<VideoDownloadServiceConfiguration> configuration)
        {
            _configuration = configuration.Value;
        }

        public async Task<RunResult<string>> DownloadVideosAudio(string url)
        {
            var downloader = new YoutubeDL()
            {
                YoutubeDLPath = _configuration.YoutubeDlBinaryPath,
                FFmpegPath = _configuration.FfmpegBinaryPath,
                RestrictFilenames = true,
                OutputFileTemplate = $"{Guid.NewGuid()}"         
            };

            var result = await downloader.RunAudioDownload(url, YoutubeDLSharp.Options.AudioConversionFormat.Opus);
            return result;
        }

        public void CleanVideoDownload(string fileDirectory)
            => File.Delete(fileDirectory);
    }
}
