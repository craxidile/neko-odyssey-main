using NekoOdyssey.Scripts.Database.Domains.SaveV001.GameSettingsEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.SaveV001.GameSettingsEntity.Repo
{
    public class GameSettingsV001Repo : Repository<GameSettingsV001, int, SQLiteConnection>
    {
        public GameSettingsV001Repo(IDbContext<SQLiteConnection> dbContext) : base(dbContext)
        {
        }

        public GameSettingsV001 Load()
        {
            return _dbContext.Context
                .Table<GameSettingsV001>()
                .FirstOrDefault();
        }
    }
}