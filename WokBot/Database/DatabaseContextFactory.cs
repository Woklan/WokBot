using EFCore.AutomaticMigrations;
using Microsoft.EntityFrameworkCore;
using WokBot.Interfaces;

namespace WokBot.Database
{
    public class DatabaseContextFactory : IDatabaseContextFactory
    {
        public DatabaseContextFactory(DatabaseContext context)
        {
            context.MigrateToLatestVersion();
        }

        public DatabaseContext Create()
        {
            var dbContext = new DatabaseContext(new Microsoft.EntityFrameworkCore.DbContextOptions<DatabaseContext>());

            return dbContext;
        }
    }
}
