using Discord;
using Discord.Commands;
using Discord.Rest;
using FFmpeg.NET;
using System;
using System.Threading.Tasks;
using WokBot.Classes;
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
                IVoiceChannel channel = null;

                channel = channel ?? (Context.User as IGuildUser)?.VoiceChannel;

                if (channel == null) return;

                if (await Program.utility.CheckBotInVoiceChat(channel))
                {
                    Console.WriteLine("ALERT: Bot was already in channel");
                    return;
                }

                youtubeInterface data;

                RestUserMessage message = await Context.Channel.SendMessageAsync("Generating Search Term");

                do
                {
                    string random_combination = Program.utility.GenerateRandomString(5);

                    await Context.Channel.SendMessageAsync("I searched for: " + random_combination + ".");

                    string url = "https://www.googleapis.com/youtube/v3/search?key=" + Program.resourcesInterface.youtube + "&maxResults=1&part=snippet&type=video&q=" + random_combination;

                    data = await Program.utility.ApiCall<youtubeInterface>(url);

                    if(data.Items.Count == 0)
                    {
                        Console.WriteLine("WARNING: Generated String couldn't get a Video.");
                    }
                } while (data.Items.Count == 0);
                
                Console.WriteLine("SUCCESS: Found a Youtube Video");
                await message.ModifyAsync(x => x.Content = "Downloading Youtube Video");

                string video_id = data.Items[0].Id.VideoId;

                string video_url = "https://www.youtube.com/watch?v=" + video_id;

                YoutubeWrapper youtube = new YoutubeWrapper(video_url, true);

                string fileName = await youtube.download();

                await Program.utility.PlayAudio(channel, fileName);

                youtube.delete();

                await Context.Channel.SendMessageAsync("https://www.youtube.com/watch?v=" + video_id);

                Console.WriteLine("BIG SUCCESSS");
            }catch(Exception e)
            {
                Console.WriteLine("Error: {0}", e);
            }
            
        }
    }
}
