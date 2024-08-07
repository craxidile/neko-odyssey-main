using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.RoutineConditionEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.RoutineConditionEntity.Repo
{
    public class RoutineConditionRepo : Repository<RoutineCondition, int, SQLiteConnection>
    {
        public RoutineConditionRepo(IDbContext<SQLiteConnection> dbContext) : base(dbContext)
        {
        }

        public ICollection<RoutineCondition> List()
        {
            return _dbContext.Context.Table<RoutineCondition>()
                .OrderBy(rc => rc.Id)
                .ToList();
        }

        public RoutineCondition FindById(int id)
        {
            return _dbContext.Context.Table<RoutineCondition>()
                .FirstOrDefault(rc => rc.Id == id);
        }

        public ICollection<RoutineCondition> ListByRoutineId(int routineId)
        {
            return _dbContext.Context.Table<RoutineCondition>()
                .Where(rc => rc.RoutineId == routineId)
                .OrderBy(rc => rc.Id)
                .ToList();
        }
    }
}