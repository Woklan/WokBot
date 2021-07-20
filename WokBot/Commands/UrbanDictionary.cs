using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using WokBot.Interfaces;

namespace WokBot.Commands
{
    public class UrbanDictionary : ModuleBase<SocketCommandContext>
    {
        [Command("urban")]

        public async Task SayAsync(string search_term)
        {
            string url = "http://api.urbandictionary.com/v0/define?term=" + search_term;

            urbanInterface result = await Program.utility.ApiCall<urbanInterface>(url);

            if (result.data.Count != 0)
            {
                var embed = new EmbedBuilder();
                embed.Title = search_term;
                embed.AddField("Definition", result.data[0].Definition);
                embed.AddField("Example", result.data[0].Example);
                embed.WithUrl(result.data[0].Permalink);
                embed.WithColor(Color.Blue);
                embed.WithFooter(footer => footer.Text = "Submitted by: " + result.data[0].Author);
                await ReplyAsync(embed: embed.Build());
                await Context.Channel.SendMessageAsync("POG!");
            }
            else
            {
                await Context.Channel.SendMessageAsync("I found no definition for the term: " + search_term + ".");
            }
        }
    }
}
