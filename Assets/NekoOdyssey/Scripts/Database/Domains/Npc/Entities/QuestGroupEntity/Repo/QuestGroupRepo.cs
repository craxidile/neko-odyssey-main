using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestGroupEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestGroupEntity.Repo
{
    public class QuestGroupRepo : Repository<QuestGroup, int, SQLiteConnection>
    {
        public QuestGroupRepo(IDbContext<SQLiteConnection> dbContext) : base(dbContext)
        {
        }

        public ICollection<QuestGroup> List()
        {
            return _dbContext.Context.Table<QuestGroup>()
                .OrderBy(qg => qg.Id)
                .ToList();
        }

        public QuestGroup FindById(int id)
        {
            return _dbContext.Context.Table<QuestGroup>()
                .FirstOrDefault(qg => qg.Id == id);
        }
    }
}