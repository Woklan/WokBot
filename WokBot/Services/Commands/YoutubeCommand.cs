using NetCord.Gateway;
using NetCord.Gateway.Voice;
using NetCord.Logging;
using NetCord.Services.Commands;
using System;
using System.Threading.Tasks;
using WokBot.Interfaces;

namespace WokBot.Services.Commands
{
    public class YoutubeCommand : CommandModule<CommandContext>
    {
        private readonly IVideoDownloadService _videoDownloadService;
        private readonly IFfmpegService _ffmpegService;
        public YoutubeCommand(IVideoDownloadService videoDownloaderService, IFfmpegService ffmpegService)
        {
            _videoDownloadService = videoDownloaderService;
            _ffmpegService = ffmpegService;
        }

        [Command("youtube")]
        public async Task SayAsync(string searchTerm)
        {
            if(!Uri.IsWellFormedUriString(searchTerm, UriKind.Absolute))
            {
                await Context.Channel.SendMessageAsync("Invalid Url!");
                return;
            }

            var guild = Context.Guild;
            if(!guild.VoiceStates.TryGetValue(Context.User.Id, out var voiceState))
            {
                await Context.Channel.SendMessageAsync("You are not connected to any voice channel!");
                return;
            }

            var client = Context.Client;

            using var video = await _videoDownloadService.DownloadVideosAudioAsync(searchTerm);

            using var voiceClient = await client.JoinVoiceChannelAsync(
                guild.Id,
                voiceState.ChannelId.GetValueOrDefault(),
                new VoiceClientConfiguration
                {
                    Logger = new ConsoleLogger()
                });

            using var outputStream = voiceClient.CreateOutputStream();

            using var opusEncodedStream = new OpusEncodeStream(outputStream, PcmFormat.Short, VoiceChannels.Stereo, OpusApplication.Audio);

            await voiceClient.StartAsync();

            await voiceClient.EnterSpeakingStateAsync(new SpeakingProperties(SpeakingFlags.Microphone));

            using (var ffmpeg = _ffmpegService.CreateFfmpegInstance(video.FilePath))
            {
                await ffmpeg.StandardOutput.BaseStream.CopyToAsync(opusEncodedStream);
            }

            await opusEncodedStream.FlushAsync();
            await voiceClient.CloseAsync();

            await client.UpdateVoiceStateAsync(new VoiceStateProperties(guild.Id, null));
        }
    }
}
