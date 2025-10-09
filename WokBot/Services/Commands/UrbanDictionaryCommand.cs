using Discord;
using Discord.Commands;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WokBot.Models;

namespace WokBot.Services.Commands
{
    public class UrbanDictionaryCommand : ModuleBase<SocketCommandContext>
    {
        private const string UrbanDictionaryApiUrl = "http://api.urbandictionary.com/v0/define?term=";
        private const string Definition = "Definition";
        private const string Example = "Example";

        private readonly IHttpClientFactory _httpClientFactory;

        public UrbanDictionaryCommand(IHttpClientFactory httpClientFactory) 
        {
            _httpClientFactory = httpClientFactory;
        }

        [Command("urban")]
        public async Task SayAsync(string searchTerm)
        {
            var searchUrl = UrbanDictionaryApiUrl + searchTerm;

            using var client = _httpClientFactory.CreateClient();

            var result = await client.GetFromJsonAsync<Root>(searchUrl);

            if (!result?.list.Any() ?? false)
            {
                await Context.Channel.SendMessageAsync($"I found no definition for the term: {searchTerm}.");
            }

            if (!result.list.Any())
            {
                return;
            }

            var definition = result.list.First();

            var embed = GenerateEmbed(searchTerm, definition);
            await ReplyAsync(embed: embed);
        }

        private Embed GenerateEmbed(string searchTerm, List definition)
        {
            var embed = new EmbedBuilder
            {
                Title = searchTerm,
            };

            embed.AddField(Definition, definition.definition);
            embed.AddField(Example, definition.example);
            embed.WithUrl(definition.permalink);
            embed.WithColor(Color.Blue);
            embed.WithFooter(footer => footer.Text = "Submitted by: " + definition.author);

            return embed.Build();
        }
    }
}
