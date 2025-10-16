using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WokBot.Interfaces;
using WokBot.Models.Config;
using WokBot.Services.Commands;

namespace WokBot.Classes
{
    public class CommandHandler : ICommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly ICommandServiceWrapper _commandService;
        private readonly IServiceProvider _serviceProvider;
        private readonly CommandHandlerConfiguration _configuration;

        private readonly List<Type> _commands = new()
        {
            typeof(PingPongCommand),
            typeof(UrbanDictionaryCommand),
            typeof(YoutubeCommand)
        };
        
        private int _argPos = 0;

        public CommandHandler(DiscordSocketClient client, ICommandServiceWrapper commandService, IServiceProvider serviceProvider, IOptions<CommandHandlerConfiguration> configuration)
        {
            _commandService = commandService;
            _client = client;
            _serviceProvider = serviceProvider;
            _configuration = configuration.Value;
        }

        public async Task InstallCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;

            await RegisterCommandsAsync();
        }

        public async Task HandleCommandAsync(SocketMessage messageParam)
        {
            if (messageParam is not SocketUserMessage message)
            {
                return;
            }

            var isBot = message.Author.IsBot;
            if (isBot)
            {
                return;
            }

            var commandToken = _configuration.CommandToken[0];
            var hasAmpersand = message.HasCharPrefix(commandToken, ref _argPos);
            if (!hasAmpersand)
            {
                return;
            }

            var context = new SocketCommandContext(_client, message);
            await _commandService.ExecuteAsync(context, _argPos, _serviceProvider);
        }

        private async Task RegisterCommandsAsync()
        {
            foreach(var command in _commands)
            {
                await _commandService.AddModuleAsync(command, _serviceProvider);
            }
        }
    }
}
