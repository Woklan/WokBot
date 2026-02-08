using Microsoft.Extensions.DependencyInjection;

namespace WokBotDatabase
{
    public static class Extensions
    {
        public static IServiceCollection AddDatabaseServices(this IServiceCollection services)
        {
            services.AddSingleton<DatabaseContext>();
            services.AddSingleton<IDatabaseContextFactory, DatabaseContextFactory>();

            return services;
        }
    }
}
