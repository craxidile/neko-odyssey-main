using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestEntity.Repo
{
    public class QuestRepo : Repository<Quest, int, SQLiteConnection>
    {
        public QuestRepo(IDbContext<SQLiteConnection> dbContext) : base(dbContext)
        {
        }

        public ICollection<Quest> List()
        {
            return _dbContext.Context.Table<Quest>()
                .OrderBy(q => q.Id)
                .ToList();
        }

        public Quest FindById(int id)
        {
            return _dbContext.Context.Table<Quest>()
                .FirstOrDefault(q => q.Id == id);
        }

        public Quest FindByCode(string code)
        {
            return _dbContext.Context.Table<Quest>()
                .FirstOrDefault(q => q.Code == code);
        }

        public ICollection<Quest> ListByQuestGroupId(int questGroupId)
        {
            return _dbContext.Context.Table<Quest>()
                .Where(q => q.QuestGroupId == questGroupId)
                .OrderBy(q => q.Id)
                .ToList();
        }
    }
}