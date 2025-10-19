using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using WokBot.Interfaces;
using WokBot.Models.Config;
using YoutubeDLSharp;

namespace WokBot.Services.VideoDownloadService
{
    public class VideoDownloadService : IVideoDownloadService
    {
        private VideoDownloadServiceConfiguration _configuration;

        public VideoDownloadService(IOptions<VideoDownloadServiceConfiguration> configuration)
        {
            _configuration = configuration.Value;
        }

        public async Task<VideoDownloadWrapper> DownloadVideosAudioAsync(string url)
        {
            var downloader = new YoutubeDL()
            {
                YoutubeDLPath = _configuration.YoutubeDlBinaryPath,
                FFmpegPath = _configuration.FfmpegBinaryPath,
                RestrictFilenames = true,
                OutputFileTemplate = $"{Guid.NewGuid()}"         
            };

            var result = await downloader.RunAudioDownload(url, YoutubeDLSharp.Options.AudioConversionFormat.Opus);

            return new VideoDownloadWrapper(result.Data);
        }
    }
}
