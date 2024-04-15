using SQLite4Unity3d;

namespace SpatiumInteractive.Libraries.Unity.GRU.Contracts
{
    public interface IDbContext<out T> where T : SQLiteConnection
    {
        T Context { get; }
    }
}
