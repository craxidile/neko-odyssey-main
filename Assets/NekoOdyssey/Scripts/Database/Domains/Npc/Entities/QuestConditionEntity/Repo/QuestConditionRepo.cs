using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestConditionEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestConditionEntity.Repo
{
    public class QuestConditionRepo : Repository<QuestCondition, int, SQLiteConnection>
    {
        public QuestConditionRepo(IDbContext<SQLiteConnection> dbContext) : base(dbContext)
        {
        }

        public ICollection<QuestCondition> List()
        {
            return _dbContext.Context.Table<QuestCondition>()
                .OrderBy(qc => qc.Id)
                .ToList();
        }

        public QuestCondition FindById(int id)
        {
            return _dbContext.Context.Table<QuestCondition>()
                .FirstOrDefault(qc => qc.Id == id);
        }

        public ICollection<QuestCondition> ListByQuestId(int questId)
        {
            return _dbContext.Context.Table<QuestCondition>()
                .Where(qc => qc.QuestId == questId)
                .OrderBy(qc => qc.Id)
                .ToList();
        }
    }
}