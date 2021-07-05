using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.WebSocket;
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

        [Command("youtube", RunMode = RunMode.Async)]
        public async Task SayAsync()
        {
            try
            {
                youtubeInterface data;
                do
                {
                    string random_combination = Program.utility.GenerateRandomString(5);

                    string url = "https://www.googleapis.com/youtube/v3/search?key=" + Program.resourcesInterface.youtube + "&maxResults=1&part=snippet&type=video&q=" + random_combination;

                    data = await Program.utility.ApiCall<youtubeInterface>(url);

                    if(data.Items.Count == 0)
                    {
                        Console.WriteLine("WARNING: Generated String couldn't get a Video.");
                    }
                } while (data.Items.Count == 0);
                
                Console.WriteLine("SUCCESS: Found a Youtube Video");

                string video_id = data.Items[0].Id.VideoId;

                await Program.utility.YoutubeDownload(video_id);

                Console.WriteLine("Youtube Success");

                await ModifyVideo();

                IVoiceChannel channel = null;

                channel = channel ?? (Context.User as IGuildUser)?.VoiceChannel;

                if (channel == null)
                {
                    return;
                }

                await Program.utility.PlayAudio(channel);

                if(File.Exists(Program.resourcesInterface.video_output + "video.mp3"))
                {
                    File.Delete(Program.resourcesInterface.video_output + "video.mp3");
                }

                if(File.Exists(Program.resourcesInterface.video_output + "video.mp3"))
                {
                    File.Delete(Program.resourcesInterface.video_output + "video2.mp3");
                }

                await Context.Channel.SendMessageAsync("https://www.youtube.com/watch?v=" + video_id);

                Console.WriteLine("BIG SUCCESSS");
            }catch(Exception e)
            {
                Console.WriteLine("Error: {0}", e);
            }
            
        }
    }
}
