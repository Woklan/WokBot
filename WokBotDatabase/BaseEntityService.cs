namespace WokBotDatabase
{
    public class BaseEntityService<T> : IBaseEntityService<T>
    {
        private readonly IDatabaseContextFactory _databaseContextFactory;

        public BaseEntityService(IDatabaseContextFactory databaseContextFactory)
        {
            _databaseContextFactory = databaseContextFactory;
        }

        public async Task AddEntitiesAsync(List<T> entitiesToBeAdded)
        {
            using var databaseContext = _databaseContextFactory.Create();

            await databaseContext.AddRangeAsync(entitiesToBeAdded);

            await databaseContext.SaveChangesAsync();
        }

        public async Task DeleteEntityAsync(int id)
        {
            using var databaseContext = _databaseContextFactory.Create();

            var entityToDelete = await databaseContext.FindAsync(typeof(T), [id]);
            if(entityToDelete is null)
            {
                return;
            }

            databaseContext.Remove(entityToDelete);

            await databaseContext.SaveChangesAsync();
        }
    }
}
