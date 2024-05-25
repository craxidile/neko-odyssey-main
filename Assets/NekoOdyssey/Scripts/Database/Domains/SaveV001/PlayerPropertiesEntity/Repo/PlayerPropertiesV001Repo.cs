using NekoOdyssey.Scripts.Database.Domains.SaveV001.PlayerPropertiesEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.SaveV001.PlayerPropertiesEntity.Repo
{
    public class PlayerPropertiesV001Repo : Repository<PlayerPropertiesV001, int, SQLiteConnection>
    {
        public PlayerPropertiesV001Repo(IDbContext<SQLiteConnection> dbContext) : base(dbContext)
        {
        }

        public PlayerPropertiesV001 Load()
        {
            return _dbContext.Context
                .Table<PlayerPropertiesV001>()
                .FirstOrDefault();
        }
    }
}