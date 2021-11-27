using Discord;
using Discord.Commands;
using Discord.Rest;
using FFmpeg.NET;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WokBot.Classes;
using WokBot.Interfaces;

namespace WokBot.Commands
{
    public class YoutubeModule : ModuleBase<SocketCommandContext>
    {
        private static int _logNumber;
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
        public async Task SayAsync(string video)
        {
            _logNumber = Program.generateLogNum();

            Program.logger.LogInformation(_logNumber, Context.User + " used the Youtube Command.");

            try
            {
                IVoiceChannel channel = null;

                channel = channel ?? (Context.User as IGuildUser)?.VoiceChannel;

                if (channel == null) return;

                if (await Program.utility.CheckBotInVoiceChat(channel))
                {
                    Program.logger.LogError(_logNumber, "Bot was already in channel.");
                    return;
                }

                youtubeInterface data;

                string video_url = "";
                string video_id = "";

                YoutubeWrapper youtube;

                if(video == "-r")
                {
                    RestUserMessage message = await Context.Channel.SendMessageAsync("Generating Search Term");

                    do
                    {
                        string random_combination = Program.utility.GenerateRandomString(5);

                        await Context.Channel.SendMessageAsync("I searched for: " + random_combination + ".");

                        string url = "https://www.googleapis.com/youtube/v3/search?key=" + Program.resourcesInterface.youtube + "&maxResults=1&part=snippet&type=video&q=" + random_combination;

                        data = await Program.utility.ApiCall<youtubeInterface>(url);

                        if (data.Items.Count == 0)
                        {
                            Program.logger.LogInformation(_logNumber, "Generated String couldn't get a video.");
                        }
                    } while (data.Items.Count == 0);

                    Program.logger.LogInformation(_logNumber, "Found a Youtube Video");

                    await message.ModifyAsync(x => x.Content = "Downloading Youtube Video");

                    video_id = data.Items[0].Id.VideoId;

                    video_url = "https://www.youtube.com/watch?v=" + video_id;

                    youtube = new YoutubeWrapper(video_url, true);
                }
                else
                {
                    video_url = video;

                    youtube = new YoutubeWrapper(video_url);
                }

                string fileName = await youtube.download();

                await Program.utility.PlayAudio(channel, fileName, _logNumber);

                youtube.delete();

                if(video == "-r")
                {
                    await Context.Channel.SendMessageAsync("https://www.youtube.com/watch?v=" + video_id);
                }

                Program.logger.LogInformation(_logNumber, Context.User + "'s Youtube Command was completed.");
            }catch(Exception e)
            {
                Program.logger.LogError(_logNumber, e.ToString());
            }
            
        }
    }
}
