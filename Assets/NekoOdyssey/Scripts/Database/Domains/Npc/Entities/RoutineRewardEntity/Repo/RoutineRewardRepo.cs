using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.QuestRewardEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Npc.Entities.RoutineRewardEntity.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SpatiumInteractive.Libraries.Unity.GRU.Domain;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.RoutineRewardEntity.Repo
{
    public class RoutineRewardRepo : Repository<RoutineReward, int, SQLiteConnection>
    {
        public RoutineRewardRepo(IDbContext<SQLiteConnection> dbContext) : base(dbContext)
        {
        }

        public ICollection<RoutineReward> List()
        {
            return _dbContext.Context.Table<RoutineReward>()
                .OrderBy(rr => rr.Id)
                .ToList();
        }

        public RoutineReward FindById(int id)
        {
            return _dbContext.Context.Table<RoutineReward>()
                .FirstOrDefault(rr => rr.Id == id);
        }

        public ICollection<RoutineReward> ListByRoutineId(int routineId)
        {
            return _dbContext.Context.Table<RoutineReward>()
                .Where(rr => rr.RoutineId == routineId)
                .OrderBy(rr => rr.Id)
                .ToList();
        }
    }
}