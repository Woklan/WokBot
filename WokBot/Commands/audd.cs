using Discord;
using Discord.Commands;
using Discord.Rest;
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

            RestUserMessage message = await Context.Channel.SendMessageAsync("audd: Searching for song...");

            try
            {
                await Program.utility.YoutubeDownloadLink(search_video, "audd.mp3");
            }catch(Exception e)
            {
                Console.WriteLine("Error: {0}", e);
            }

            await message.ModifyAsync(x => x.Content = "audd: Clipping Song...");

            await Program.utility.CutVideo("audd.mp3", "audd2.mp3", 0, 15);

            await message.ModifyAsync(x => x.Content = "audd: Comparing Audio...");

            FileStream data = File.OpenRead(Program.resourcesInterface.video_output + "audd2.mp3");

            List<(string, string)> param = new List<(string, string)>();

            param.Add(("api_token", Program.resourcesInterface.audD));

            auddInterface response = await Program.utility.UploadAsync<auddInterface>("https://api.audd.io", "audd2.mp3", data, param);

            if (response.status == "success")
            {
                if (response.result == null)
                {
                    await message.ModifyAsync(x => x.Content = "audd: I could not find the song specified!");
                }
                else
                {
                    Console.WriteLine("POG");
                    string description = ("Artist: " + response.result.artist + "\nAlbum: " + response.result.album + "\nRelease Date: " + response.result.release_date);
                    var embed = new EmbedBuilder();
                    embed.Title = response.result.title;
                    embed.Url = response.result.song_link;
                    // This is causing issue
                    if (response.result.spotify != null && response.result.spotify.album != null && response.result.spotify.album.images.Count > 0 && response.result.spotify.album.images[0].url != null)
                    {
                        embed.WithThumbnailUrl(response.result.spotify.album.images[0].url);
                    }
                    embed.Description = description;
                    embed.WithColor(Color.Red);
                    embed.WithFooter(footer => footer.Text = "Audio Recognition used is audd | https://audd.io/");
                    await ReplyAsync(embed: embed.Build());
                    await message.DeleteAsync();
                }
            }
            else
            {
                await message.ModifyAsync(x => x.Content = Program.utility.audDError(response.error.error_code));
            }

            File.Delete(Program.resourcesInterface.video_output + "audd.mp3");
            File.Delete(Program.resourcesInterface.video_output + "audd2.mp3");
        }
    }
}
