using WokBot.Database;

namespace WokBot.Interfaces
{
    public interface IDatabaseContextFactory
    {
        DatabaseContext Create();
    }
}
