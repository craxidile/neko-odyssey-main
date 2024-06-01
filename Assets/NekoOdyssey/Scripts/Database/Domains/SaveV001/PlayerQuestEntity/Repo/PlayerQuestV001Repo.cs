using System.Linq;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.PlayerQuestEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.SaveV001.PlayerQuestEntity.Repo
{
    public class PlayerQuestV001Repo : Repository<PlayerQuestV001, int, SQLiteConnection>
    {
        public PlayerQuestV001Repo(IDbContext<SQLiteConnection> dbContext) : base(dbContext)
        {
        }

        public PlayerQuestV001 FindByQuestCode(string questCode)
        {
            return _dbContext.Context
                .Table<PlayerQuestV001>()
                .FirstOrDefault(pq => pq.QuestCode == questCode);
        }
    }
}