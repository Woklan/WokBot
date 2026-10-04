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
            var searchUrl = $"{_configuration.ApiUrl}{searchTerm}";

            using var httpClient = _httpClientFactory.CreateClient();

            var result = await httpClient.GetFromJsonAsync<UrbanDictionaryResponse>(searchUrl);

            if (!result?.UrbanDictionaryDefinitions.Any() ?? false)
            {
                await Context.Channel.SendMessageAsync($"I found no definition for the term: {searchTerm}.");
                return null;
            }

            var definition = result.UrbanDictionaryDefinitions.First();

            var embed = GenerateEmbed(searchTerm, definition);

            var message = new MessageProperties();
            message.AddEmbeds([embed]);

            return message;
        }

        private EmbedProperties GenerateEmbed(string searchTerm, UrbanDictionaryDefinition definition)
        {
            var color = new Color(byte.MinValue, byte.MinValue, byte.MaxValue);
            var embed = new EmbedProperties()
                .WithTitle(searchTerm)
                .AddFields([
                    new EmbedFieldProperties
                    {
                        Name = "Definition",
                        Value = definition.Definition
                    },
                    new EmbedFieldProperties
                    {
                        Name = "Example",
                        Value = definition.Example
                    }
                    ])
                .WithUrl(definition.Permalink)
                .WithColor(color)
                .WithFooter(new EmbedFooterProperties
                {
                    Text = "Submitted by: " + definition.Author
                });

            return embed;
        }
    }
}
