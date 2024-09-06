using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.PlayerQuestEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.PlayerQuestGroupEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.SaveV001.PlayerQuestGroupEntity.Repo
{
    public class PlayerQuestGroupV001Repo : Repository<PlayerQuestV001, int, SQLiteConnection>
    {
        public PlayerQuestGroupV001Repo(IDbContext<SQLiteConnection> dbContext) : base(dbContext)
        {
        }

        public ICollection<PlayerQuestGroupV001> List()
        {
            return _dbContext.Context
                .Table<PlayerQuestGroupV001>()
                .OrderBy(pqg => pqg.Id)
                .ToList();
        }

        public PlayerQuestGroupV001 FindByQuestGroupCode(string questGroupCode)
        {
            return _dbContext.Context
                .Table<PlayerQuestGroupV001>()
                .FirstOrDefault(pqg => pqg.QuestGroupCode == questGroupCode);
        }
    }
}