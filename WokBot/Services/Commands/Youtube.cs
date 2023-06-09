using Discord;
using Discord.Audio;
using Discord.Commands;
using System.Threading.Tasks;

namespace WokBot.Services.Commands
{
    public class Youtube : ModuleBase<SocketCommandContext>
    {
        private readonly VideoDownloadService _videoDownloadService;
        private readonly FfmpegService _ffmpegService;
        public Youtube(VideoDownloadService videoDownloaderService, FfmpegService ffmpegService)
        {
            _videoDownloadService = videoDownloaderService;
            _ffmpegService = ffmpegService;
        }

        [Command("youtube", RunMode = RunMode.Async)]
        public async Task SayAsync(string searchTerm, IVoiceChannel channel = null)
        {
            channel ??= (Context.User as IGuildUser)?.VoiceChannel;
            if (channel is null)
            {
                await Context.Channel.SendMessageAsync("User must be in a voice channel!");
                return;
            }

            var downloadResult = await _videoDownloadService.DownloadVideosAudio(searchTerm);
            if (!downloadResult.Success)
            {
                await Context.Channel.SendMessageAsync("Something went wrong!");
                return;
            }

            await PlayAudioInChannel(channel, downloadResult.Data);

            _videoDownloadService.CleanVideoDownload(downloadResult.Data);
        }

        public async Task PlayAudioInChannel(IVoiceChannel channel, string videoOutputDirectory)
        {
            var audioClient = await channel.ConnectAsync();

            using var ffmpegInstance = _ffmpegService.CreateFfmpegInstance(videoOutputDirectory);
            using var output = ffmpegInstance.StandardOutput.BaseStream;
            using var discord = audioClient.CreatePCMStream(AudioApplication.Mixed);

            try
            {
                await output.CopyToAsync(discord);
            }
            finally
            {
                await discord.FlushAsync();
            }

            await channel.DisconnectAsync();

            _ffmpegService.CloseFfmpegInstance(ffmpegInstance);
        }
    }
}
