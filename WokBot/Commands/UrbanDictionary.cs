using Discord.Commands;
using System;
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

            urban_dictionary result = await Program.utility.ApiCall<urban_dictionary>(url);

            if (result.data.Count != 0)
            {
                await Context.Channel.SendMessageAsync("POG!");
            }
            else
            {
                await Context.Channel.SendMessageAsync("Empty!");
            }
        }
    }
}
