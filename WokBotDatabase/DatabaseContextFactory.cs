using EFCore.AutomaticMigrations;
using Microsoft.EntityFrameworkCore;

namespace WokBotDatabase
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
