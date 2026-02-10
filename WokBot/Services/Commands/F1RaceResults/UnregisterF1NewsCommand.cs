using NetCord.Rest;
using NetCord.Services.Commands;
using System.Linq;
using System.Threading.Tasks;
using WokBot.Interfaces;
using WokBot.Models.Database;

namespace WokBot.Services.Commands.F1RaceResults
{
    public class UnregisterF1NewsCommand : CommandModule<CommandContext>
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;

        public UnregisterF1NewsCommand(IDatabaseContextFactory databaseContextFactory)
        {
            _databaseContextFactory = databaseContextFactory;
        }

        [Command("unregister-f1")]
        public async Task<MessageProperties> UnregisterChannelAsync()
        {
            var guild = await Context.Guild.GetAsync();
            var channel = await Context.Channel.GetAsync();

            using var databaseContext = _databaseContextFactory.Create();

            var subscriptions = databaseContext.Set<F1ChannelUpdateSubscription>();

            var subscriptionsToDelete = subscriptions.Where(x => x.GuildId == guild.Id && x.ChannelId == channel.Id).ToList();

            subscriptions.RemoveRange(subscriptionsToDelete);

            await databaseContext.SaveChangesAsync();

            return new MessageProperties
            {
                Content = "This channel has been successfully unregistered from receiving F1 news updates."
            };
        }
    }
}
