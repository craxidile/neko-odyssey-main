using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogConditionOptionEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogConditionOptionEntity.Repo
{
    public class DialogConditionOptionRepo : Repository<DialogConditionOption, int, SQLiteConnection>
    {
        public DialogConditionOptionRepo(IDbContext<SQLiteConnection> dbContext) : base(dbContext)
        {
        }
        
        public ICollection<DialogConditionOption> List()
        {
            return _dbContext.Context.Table<DialogConditionOption>()
                .OrderBy(dcc => dcc.Id)
                .ToList();
        }

        public DialogConditionOption FindById(int id)
        {
            return _dbContext.Context.Table<DialogConditionOption>()
                .FirstOrDefault(dcc => dcc.Id == id);
        }

        public ICollection<DialogConditionOption> ListByConditionId(int conditionId)
        {
            return _dbContext.Context.Table<DialogConditionOption>()
                .Where(dcc => dcc.ConditionId == conditionId)
                .OrderBy(dcc => dcc.Id)
                .ToList();
        }
    }
}