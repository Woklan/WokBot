namespace WokBotDatabase
{
    public interface IBaseEntityService<T>
    {
        Task AddEntitiesAsync(List<T> entitiesToBeAdded);
        Task DeleteEntityAsync(int id);
    }
}
