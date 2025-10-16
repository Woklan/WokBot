using Discord;
using Discord.Commands;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WokBot.Models;
using WokBot.Models.Config;

namespace WokBot.Services.Commands
{
    public class UrbanDictionaryCommand : ModuleBase<SocketCommandContext>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private UrbanDictionaryCommandConfiguration _configuration;

        public UrbanDictionaryCommand(IHttpClientFactory httpClientFactory, IOptions<UrbanDictionaryCommandConfiguration> configuration) 
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration.Value;
        }

        [Command("urban")]
        public async Task SayAsync(string searchTerm)
        {
            var searchUrl = $"{_configuration.UrbanDictionaryApiUrl}{searchTerm}";

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

            embed.AddField(_configuration.DefinitionTitle, definition.definition);
            embed.AddField(_configuration.ExampleTitle, definition.example);
            embed.WithUrl(definition.permalink);
            embed.WithColor(Color.Blue);
            embed.WithFooter(footer => footer.Text = "Submitted by: " + definition.author);

            return embed.Build();
        }
    }
}
