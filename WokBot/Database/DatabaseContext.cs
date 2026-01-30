using Microsoft.EntityFrameworkCore;
using WokBot.Models.Database;

namespace WokBot.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
           : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=wokbot.db");
        }

        public DbSet<F1ChannelUpdateSubscription> F1ChannelUpdateSubscriptions { get; set; }
        public DbSet<LatestRaceData> LatestRaceDatas { get; set; }
    }
}
