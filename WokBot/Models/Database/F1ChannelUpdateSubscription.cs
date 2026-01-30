using System.ComponentModel.DataAnnotations;

namespace WokBot.Models.Database
{
    public class F1ChannelUpdateSubscription
    {
        [Key]
        public int Id { get; set; }

        public ulong GuildId { get; set; }

        public ulong ChannelId { get; set; }
    }
}
