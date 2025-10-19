using Microsoft.Extensions.Options;
using NetCord;
using NetCord.Rest;
using NetCord.Services.Commands;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WokBot.Models;
using WokBot.Models.Config;

namespace WokBot.Services.Commands
{
    public class UrbanDictionaryCommand : CommandModule<CommandContext>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private UrbanDictionaryCommandConfiguration _configuration;

        public UrbanDictionaryCommand(IHttpClientFactory httpClientFactory, IOptions<UrbanDictionaryCommandConfiguration> configuration) 
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration.Value;
        }

        [Command("urban")]
        public async Task<MessageProperties> SayAsync(string searchTerm)
        {
            var searchUrl = $"{_configuration.UrbanDictionaryApiUrl}{searchTerm}";

            using var httpClient = _httpClientFactory.CreateClient();

            var result = await httpClient.GetFromJsonAsync<Root>(searchUrl);

            if (!result?.list.Any() ?? false)
            {
                await Context.Channel.SendMessageAsync($"I found no definition for the term: {searchTerm}.");
            }

            if (!result.list.Any())
            {
                return null;
            }

            var definition = result.list.First();

            var embed = GenerateEmbed(searchTerm, definition);

            var message = new MessageProperties();
            message.AddEmbeds([embed]);

            return message;
        }

        private EmbedProperties GenerateEmbed(string searchTerm, List definition)
        {
            var color = new Color(byte.MinValue, byte.MinValue, byte.MaxValue);
            var embed = new EmbedProperties()
                .WithTitle(searchTerm)
                .AddFields([
                    new EmbedFieldProperties
                    {
                        Name = _configuration.DefinitionTitle,
                        Value = definition.definition
                    },
                    new EmbedFieldProperties
                    {
                        Name = _configuration.ExampleTitle,
                        Value = definition.definition
                    }
                    ])
                .WithUrl(definition.permalink)
                .WithColor(color)
                .WithFooter(new EmbedFooterProperties
                {
                    Text = "Submitted by: " + definition.author
                });

            return embed;
        }
    }
}
