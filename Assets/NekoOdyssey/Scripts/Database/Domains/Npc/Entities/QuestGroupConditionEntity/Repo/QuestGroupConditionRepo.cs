using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestGroupConditionEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestGroupConditionEntity.Repo
{
    public class QuestGroupConditionRepo : Repository<QuestGroupCondition, int, SQLiteConnection>
    {
        public QuestGroupConditionRepo(IDbContext<SQLiteConnection> dbContext) : base(dbContext)
        {
        }

        public ICollection<QuestGroupCondition> List()
        {
            return _dbContext.Context.Table<QuestGroupCondition>()
                .OrderBy(qgc => qgc.Id)
                .ToList();
        }

        public QuestGroupCondition FindById(int id)
        {
            return _dbContext.Context.Table<QuestGroupCondition>()
                .FirstOrDefault(qgc => qgc.Id == id);
        }

        public ICollection<QuestGroupCondition> ListByQuestGroupId(int questGroupId)
        {
            return _dbContext.Context.Table<QuestGroupCondition>()
                .Where(qgc => qgc.QuestGroupId == questGroupId)
                .OrderBy(qgc => qgc.Id)
                .ToList();
        }
    }
}