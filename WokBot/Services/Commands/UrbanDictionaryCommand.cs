using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WokBot.Interfaces;
using WokBot.Models;

namespace WokBot.Services.Commands
{
    public class UrbanDictionaryCommand : ModuleBase<SocketCommandContext>
    {
        private const string UrbanDictionaryApiUrl = "http://api.urbandictionary.com/v0/define?term=";
        private const string Definition = "Definition";
        private const string Example = "Example";

        public UrbanDictionaryCommand(){}

        [Command("urban")]
        public async Task SayAsync(string searchTerm)
        {
            var searchUrl = UrbanDictionaryApiUrl + searchTerm;

            // TODO: USE HTTPCLIENT PROPERLY
            using var client = new HttpClient()
            {
                BaseAddress = new Uri(searchUrl)
            };

            var result = await client.GetFromJsonAsync<Root>(searchUrl);

            if (result == null || !result.list.Any())
            {
                await Context.Channel.SendMessageAsync($"I found no definition for the term: {searchTerm}.");
            }

            if (result.list.Any())
            {
                var definition = result.list.First();

                var embed = new EmbedBuilder
                {
                    Title = searchTerm
                };

                embed.AddField(Definition, definition.definition);
                embed.AddField(Example, definition.example);
                embed.WithUrl(definition.permalink);
                embed.WithColor(Color.Blue);
                embed.WithFooter(footer => footer.Text = "Submitted by: " + definition.author);
                await ReplyAsync(embed: embed.Build());
            }
        }
    }
}
