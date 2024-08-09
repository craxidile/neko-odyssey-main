using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogConditionCaseEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogConditionCaseEntity.Repo
{
    public class DialogConditionCaseRepo : Repository<DialogConditionCase, int, SQLiteConnection>
    {
        public DialogConditionCaseRepo(IDbContext<SQLiteConnection> dbContext) : base(dbContext)
        {
        }

        public ICollection<DialogConditionCase> List()
        {
            return _dbContext.Context.Table<DialogConditionCase>()
                .OrderBy(dcc => dcc.Id)
                .ToList();
        }

        public DialogConditionCase FindById(int id)
        {
            return _dbContext.Context.Table<DialogConditionCase>()
                .FirstOrDefault(dcc => dcc.Id == id);
        }

        public ICollection<DialogConditionCase> ListByConditionId(int conditionId)
        {
            return _dbContext.Context.Table<DialogConditionCase>()
                .Where(dcc => dcc.ConditionId == conditionId)
                .OrderBy(dcc => dcc.Id)
                .ToList();
        }
    }
}