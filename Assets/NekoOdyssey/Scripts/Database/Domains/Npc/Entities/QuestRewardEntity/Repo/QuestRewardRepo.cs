using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestRewardEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestRewardEntity.Repo
{
    public class QuestRewardRepo : Repository<QuestReward, int, SQLiteConnection>
    {
        public QuestRewardRepo(IDbContext<SQLiteConnection> dbContext) : base(dbContext)
        {
        }

        public ICollection<QuestReward> List()
        {
            return _dbContext.Context.Table<QuestReward>()
                .OrderBy(qr => qr.Id)
                .ToList();
        }

        public QuestReward FindById(int id)
        {
            return _dbContext.Context.Table<QuestReward>()
                .FirstOrDefault(qr => qr.Id == id);
        }

        public ICollection<QuestReward> ListByQuestId(int questId)
        {
            return _dbContext.Context.Table<QuestReward>()
                .Where(qr => qr.QuestId == questId)
                .OrderBy(qr => qr.Id)
                .ToList();
        }
    }
}