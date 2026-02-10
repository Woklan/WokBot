using Microsoft.EntityFrameworkCore;
using NetCord;
using NetCord.Rest;
using NetCord.Services.Commands;
using System.Linq;
using System.Threading.Tasks;
using WokBot.Interfaces;
using WokBot.Models.Database;

namespace WokBot.Services.Commands.F1RaceResults
{
    
    public class RegisterF1NewsCommand : CommandModule<CommandContext>
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;

        public RegisterF1NewsCommand(IDatabaseContextFactory databaseContextFactory)
        {
            _databaseContextFactory = databaseContextFactory;
        }
        
        [Command("register-f1")]
        public async Task<MessageProperties> RegisterChannel()
        {
            var guild = await Context.Guild.GetAsync();
            var channel = await Context.Channel.GetAsync();

            using var databaseContext = _databaseContextFactory.Create();

            var subscriptions = databaseContext.Set<F1ChannelUpdateSubscription>();

            var doesChannelAlreadyHaveSubscription = DoesChannelAlreadyHaveSubscription(subscriptions, guild, channel);
            if (doesChannelAlreadyHaveSubscription)
            {
                return new MessageProperties
                {
                    Content = "This channel is already registered to receive F1 news updates."
                };
            }

            await subscriptions.AddAsync(new F1ChannelUpdateSubscription
            {
                GuildId = guild.Id,
                ChannelId = channel.Id
            });

            await databaseContext.SaveChangesAsync();

            return new MessageProperties
            {
                Content = "This channel has been successfully registered to receive F1 news updates."
            };
        }

        private bool DoesChannelAlreadyHaveSubscription(DbSet<F1ChannelUpdateSubscription> subscriptions, RestGuild guild, TextChannel channel)
            => subscriptions
            .Where(x => x.GuildId == guild.Id && x.ChannelId == channel.Id)
            .Any();
    }
}
