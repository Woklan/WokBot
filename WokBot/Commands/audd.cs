using Discord.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WokBot.Interfaces;

namespace WokBot.Commands
{
    public class audd : ModuleBase<SocketCommandContext>
    {
        [Command("audd")]
        public async Task SayAsync(string search_video)
        {

            if(File.Exists(Program.resourcesInterface.video_output + "audd.mp3"))
            {
                File.Delete(Program.resourcesInterface.video_output + "audd.mp3");
            }

            if (File.Exists(Program.resourcesInterface.video_output + "audd2.mp3"))
            {
                File.Delete(Program.resourcesInterface.video_output + "audd2.mp3");
            }

            Console.WriteLine("audd Hit");
            try
            {
                await Program.utility.YoutubeDownloadLink(search_video, "audd.mp3");
            }catch(Exception e)
            {
                Console.WriteLine("Error: {0}", e);
            }
            
            Console.WriteLine("Video Downloaded");

            await Program.utility.CutVideo("audd.mp3", "audd2.mp3", 0, 15);
            Console.WriteLine("Video Cut");

            FileStream data = File.OpenRead(Program.resourcesInterface.video_output + "audd2.mp3");

            List<(string, string)> param = new List<(string, string)>();

            param.Add(("api_token", Program.resourcesInterface.audD));

            Console.WriteLine("Api Hit");
            audD response = await Program.utility.UploadAsync<audD>("https://api.audd.io", "audd2.mp3", data, param);
            Console.WriteLine("Successful Hit on API!");

            Console.WriteLine("Response: " + response.status);

            if (response.status == "success")
            {
                if (response.result == null)
                {
                    await Context.Channel.SendMessageAsync("I could not find the song");
                }
                else
                {
                    await Context.Channel.SendMessageAsync(response.result.song_link);
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync(Program.utility.audDError(response.error.error_code));
                await Context.Channel.SendMessageAsync(response.error.error_message);
            }

            File.Delete(Program.resourcesInterface.video_output + "audd.mp3");
            File.Delete(Program.resourcesInterface.video_output + "audd2.mp3");
        }
    }
}
