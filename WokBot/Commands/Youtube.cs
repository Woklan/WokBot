using Discord;
using Discord.Audio;
using Discord.Commands;
using FFmpeg.NET;
using NYoutubeDL;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WokBot.Interfaces;

namespace WokBot.Commands
{
    public class YoutubeModule : ModuleBase<SocketCommandContext>
    {
        private static Random random = new Random();
        static async Task<MediaFile> ModifyVideo()
        {
            Console.WriteLine("FFMPEG");
            var inputFile = new InputFile(@Program.resourcesInterface.video_output + "video.mp3");
            var outputFile = new OutputFile(@Program.resourcesInterface.video_output + "video2.mp3");
            var ffmpeg = new Engine(Program.resourcesInterface.ffmpeg_executable);

            var options = new ConversionOptions();

            options.CutMedia(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(5));
            var output = await ffmpeg.ConvertAsync(inputFile, outputFile, options);

            return output;
        }

        private Process CreateStream(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = Program.resourcesInterface.ffmpeg_executable,
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
            });
        }

        [Command("youtube", RunMode = RunMode.Async)]
        public async Task SayAsync()
        {
            try
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz_";
                string random_combination = new string(Enumerable.Repeat(chars, 5).Select(s => s[random.Next(s.Length)]).ToArray());

                string url = "https://www.googleapis.com/youtube/v3/search?key=" + Program.resourcesInterface.youtube + "&maxResults=1&part=snippet&type=video&q=" + random_combination;

                youtubeInterface data = await Program.utility.ApiCall<youtubeInterface>(url);

                Console.WriteLine("ApiCall Success");

                string video_id = data.Items[0].Id.VideoId;

                var youtubeDL = new YoutubeDL();

                youtubeDL.Options.FilesystemOptions.Output = Program.resourcesInterface.video_output + "video.mp3";
                youtubeDL.Options.PostProcessingOptions.ExtractAudio = true;
                youtubeDL.VideoUrl = "https://www.youtube.com/watch?v=" + video_id;

                youtubeDL.Options.PostProcessingOptions.AudioFormat = NYoutubeDL.Helpers.Enums.AudioFormat.mp3;

                // POGPGOGOGOGOEGPOGPGPOOPEGPPGOOGOG

                youtubeDL.YoutubeDlPath = Program.resourcesInterface.video_executable;

                youtubeDL.StandardOutputEvent += (sender, output) => Console.WriteLine(output);
                youtubeDL.StandardErrorEvent += (sender, errorOutput) => Console.WriteLine(errorOutput);

                await youtubeDL.DownloadAsync();

                Console.WriteLine("Youtube Success");

                await ModifyVideo();

                IVoiceChannel channel = null;

                channel = channel ?? (Context.User as IGuildUser)?.VoiceChannel;

                if(channel == null)
                {
                    return;
                }

                var audioClient = await channel.ConnectAsync();

                using (var ffmpeg = CreateStream(Program.resourcesInterface.video_output + "video2.mp3"))
                using (var output = ffmpeg.StandardOutput.BaseStream)
                using (var discord = audioClient.CreatePCMStream(AudioApplication.Mixed))
                {
                    try { await output.CopyToAsync(discord); }
                    finally { await discord.FlushAsync();  }
                }

                File.Delete(Program.resourcesInterface.video_output + "video.mp3");
                File.Delete(Program.resourcesInterface.video_output + "video2.mp3");



                Console.WriteLine("BIG SUCCESSS");
            }catch(Exception e)
            {
                Console.WriteLine("Error: {0}", e);
            }
            
        }
    }
}
